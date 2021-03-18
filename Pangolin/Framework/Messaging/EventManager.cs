using System;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using EnderPi.Framework.Caching;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Messaging.Events;
using EnderPi.Framework.Logging;
using System.Data;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// Manages everything event related.  Publishing events, subscribing to events.  Has significant state, should be a singleton in every application where it is used.
    /// </summary>
    /// <remarks>
    /// This class manages subscribing and unsubscribing, which involves storing process-local state (delegate callbacks), as well as 
    /// sending messages to the notification publisher so that the publisher routes events to this process's event queue.
    /// Pub-Sub in this framework has routing at two levels.  Events to be published are sent to the notification publisher's message queue.
    /// The notification publisher sends a copy of the message to the event queue for every application that subscribes to that event.
    /// The event manager reads from that queue, and routes a copy of the event to every delegate who has subscribed.
    /// It wraps those in try-catch blocks, so exceptions from the callbacks never influence the framework.  The errors are logged
    /// if the logger logs things at an error level.
    /// </remarks>
    public class EventManager : IEventManager
    {
        /// <summary>
        /// Where you send messages that need distributed throughout the system.
        /// </summary>
        private IMessageQueue _publishingMessageQueue;
        /// <summary>
        /// Whereever events are rolling into for the current process, pushed into here from the notification publisher.
        /// </summary>
        private IMessageQueue _receivingMessageQueue;
        /// <summary>
        /// Process-local delegates that have event subscriptions.
        /// </summary>
        private List<EventSubscription> _subscriptions;
        /// <summary>
        /// The throttled task listener that manages the threading stuff.
        /// </summary>
        private ThrottledTaskProcessor<EventMessage> _throttledMessageListener;
        /// <summary>
        /// Connection string for the database, used to read and write subscriptions.  Probably needs to be in a separate data access class.
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// Object to synchronize access to local subscriptions.
        /// </summary>
        private object _padlock = new object();
        /// <summary>
        /// Serializer for use with events.  Probably should be in a helper class, this class is too big.
        /// </summary>
        private DataContractSerializer _serializer;

        /// <summary>
        /// Logger for logging issues.
        /// </summary>
        private Logger _logger;

        /// <summary>
        /// Basic constructor.  This thing has a lot of state, as it maintains queues and DB connections.
        /// </summary>
        /// <param name="connectionString">Connection string for the database.</param>
        /// <param name="publishingQueue">The queue where events to be published are pushed.</param>
        /// <param name="receivingQueue">The event queue for the current process.</param>
        /// <param name="maxConcurrency">Maximum concurrency.</param>
        /// <param name="millisecondsToSleep">How long the manager sleeps after checking the queue and finding no messages.  Recommend 1000</param>
        /// <param name="logger">Logger object, used to log events related to message processing.</param>
        public EventManager(string connectionString, IMessageQueue publishingQueue, IMessageQueue receivingQueue, ThrottledTaskProcessorParameters parameters, Logger logger)
        {
            _logger = logger;
            _serializer = new DataContractSerializer(typeof(EventMessage), GetKnownTypes());
            _connectionString = connectionString;
            _publishingMessageQueue = publishingQueue;
            _receivingMessageQueue = receivingQueue;
            _subscriptions = new List<EventSubscription>();
            //TODO the literals below feel like general event application settings.  They need added to the constructor.            
            _throttledMessageListener = new ThrottledTaskProcessor<EventMessage>(GetNextEvent, RouteEventMessageLocally, parameters);//
            _throttledMessageListener.ExceptionOccurredInSchedulerThread += LogMessageException;
            _throttledMessageListener.ExceptionOccurredInWorkerThread += LogMessageException;
            _throttledMessageListener.MessageProcessing += LogMessageReceived;
            _throttledMessageListener.MessageProcessed += LogMessageProcessed;
            _throttledMessageListener.CleanupTaskFired += LogCleanupTaskFired;
        }

        private void LogCleanupTaskFired(object sender, ThrottledEventArgs<EventMessage> e)
        {
            _logger.Log("Cleanup task fired in Event Manager", LoggingLevel.Debug);
        }

        private void LogMessageProcessed(object sender, ThrottledEventArgs<EventMessage> e)
        {
            _logger.Log($"Message Processed in EventManager, event type {e?.Message?.GetType()}", LoggingLevel.Information);
        }

        private void LogMessageReceived(object sender, ThrottledEventArgs<EventMessage> e)
        {
            _logger.Log($"Message Received in EventManager, event type {e?.Message?.GetType()}", LoggingLevel.Information);
        }

        private void LogMessageException(object sender, ThrottledEventArgs<EventMessage> e)
        {
            if (e != null && e.Exception != null)
            {
                _logger.Log(e.Exception.ToString(), LoggingLevel.Error);
            }
        }

        /// <summary>
        /// Delegate to pull the next event out of the queue.
        /// </summary>
        /// <returns></returns>
        private EventMessage GetNextEvent()
        {
            EventMessage eventMessage = null;
            var message = _receivingMessageQueue.GetNextMessage();
            if (message != null)
            {
                eventMessage = DeserializeEventMessage(message.Body);
            }
            return eventMessage;
        }

        /// <summary>
        /// Routes the event to process-local delegates who have subscribed.
        /// </summary>
        /// <param name="message"></param>
        private void RouteEventMessageLocally(EventMessage eventMessage)
        {
            var eventType = eventMessage.GetType();
            lock (_padlock)
            {
                foreach (var subscription in _subscriptions.Where(x => x.EventType == eventType))
                {
                    Threading.Threading.ExecuteWithoutThrowing(() => subscription.EventProcessor(eventMessage));
                }
            }
        }

        /// <summary>
        /// The list of known event types.  When you create a new event, you should add it here and in event message.
        /// </summary>
        /// <returns></returns>
        private Type[] GetKnownTypes()
        {
            return new Type[] { typeof(CacheInvalidationEvent), typeof(GlobalConfigurationsUpdated), typeof(SimulationCancelledEvent), typeof(StopBackgroundTasksEvent),
                typeof(SimulationFinishedEvent)
            };
        }

        /// <summary>
        /// Publishes an event.  Subscribers in all applications who are listening will hear about it.
        /// </summary>
        /// <remarks>
        /// Serializes the event message, stuffs it into the queue.
        /// </remarks>
        /// <param name="eventMessage"></param>
        public void PublishEvent(EventMessage eventMessage, MessagePriority priority)
        {
            string messageBody = Serialize(eventMessage); 
            Message message = new Message(0, messageBody, DateTime.Now, priority);
            _publishingMessageQueue.SendMessage(message);
        }

        /// <summary>
        /// Publishes an event with a given priority.  Only use this if you actually care about sending messages out of order.
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PublishEvent(EventMessage eventMessage)
        {
            PublishEvent(eventMessage, MessagePriority.Normal);
        }

        /// <summary>
        /// Start listening for events.  Definitely needs to happen in the app, *AFTER* event subscriptions have been loaded.
        /// </summary>
        public void StartListening()
        {
            _throttledMessageListener.StartListening();
        }

        /// <summary>
        /// Stops listening.  Blocking call, indeterminate runtime.
        /// </summary>
        public void StopListening()
        {
            _throttledMessageListener.StopListening();
        }

        /// <summary>
        /// Subscribe to the given event. 
        /// </summary>
        /// <remarks>
        /// Will call the given eventProcessor with the event as a parameter whenever the given event type occurs.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventProcessor"></param>
        public void Subscribe<T>(Action<T> eventProcessor) where T : EventMessage
        {
            var eventType = typeof(T);
            lock (_padlock)
            {
                var alreadySubscribed = _subscriptions.Any(x => x.EventType == eventType);
                if (!alreadySubscribed)
                {
                    //todo this is a blocking call under a lock......
                    SubscribeThisApplicationToThisMessageType(eventType);
                }

                EventSubscription subscription = new EventSubscription() { EventType = eventType, EventProcessor = (x) => eventProcessor((T)x), TargetMethod = eventProcessor };
                _subscriptions.Add(subscription);
            }
        }

        /// <summary>
        /// Overload needed for when you load subscriptions dynamically, and do not have the event type at compile time.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventProcessor"></param>
        public void Subscribe(Type eventType, Action<EventMessage> eventProcessor)
        {
            lock (_padlock)
            {
                var alreadySubscribed = _subscriptions.Any(x => x.EventType == eventType);
                if (!alreadySubscribed)
                {
                    //todo this is a blocking call under a lock......
                    SubscribeThisApplicationToThisMessageType(eventType);
                }

                EventSubscription subscription = new EventSubscription() { EventType = eventType, EventProcessor = (x) => eventProcessor(x), TargetMethod = eventProcessor };
                _subscriptions.Add(subscription);
            }
        }

        /// <summary>
        /// Sends a cache invalidation event for subscriptions, so that the notification publisher updates.
        /// </summary>
        private void SendCacheInvalidationEvent()
        {
            CacheInvalidationEvent invalidate = new CacheInvalidationEvent();
            invalidate.CacheName = CacheNames.EventSubscriptions;
            PublishEvent(invalidate, MessagePriority.Highest);      //Cache invalidation is important
        }

        /// <summary>
        /// Unsubscribes the given delegate from the event.  Will do all unsubscriptions at once if a delegate is subscribed multiple times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventProcessor"></param>
        public void Unsubscribe<T>(Action<T> eventProcessor) where T : EventMessage
        {
            var eventType = typeof(T);
            lock (_padlock)
            {
                _subscriptions.RemoveAll(x => x.EventType == eventType && (Action<T>)x.TargetMethod == eventProcessor);

                var stillSubscribed = _subscriptions.Any(x => x.EventType == eventType);
                if (!stillSubscribed)
                {
                    UnsubscribeThisApplicationToThisMessageType(eventType.ToString());
                }
            }
        }

        /// <summary>
        /// Subscribes the current application to the given event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="receivingQueueName"></param>
        /// <remarks>
        /// Used internally by the event manager.  The event manager needs to ensure that when any delegate subscribes to an event, that the event gets 
        /// routed at both levels.  To this application by the notification publisher, and to the given delegate by the local event manager.  
        /// This method ensures the former.  The stored proc is safe - it inserts only if it doesn't exist.
        /// </remarks>
        private void SubscribeThisApplicationToThisMessageType(Type eventType)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[MessageQueue].[CreateSubscription]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = _receivingMessageQueue.Name;
                    command.Parameters.Add("@EventType", SqlDbType.VarChar, 200).Value = eventType.ToString();
                    command.ExecuteNonQuery();
                }
            }
            SendCacheInvalidationEvent();
        }

        /// <summary>
        /// Unsubscribes the current application from the given event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="receivingQueueName"></param>
        private void UnsubscribeThisApplicationToThisMessageType(string eventType)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[MessageQueue].[DeleteSubscription]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = _receivingMessageQueue.Name;
                    command.Parameters.Add("@EventType", SqlDbType.VarChar, 200).Value = eventType.ToString();
                    command.ExecuteNonQuery();
                }
            }
            SendCacheInvalidationEvent();
        }

        /// <summary>
        /// Gets all application subscriptions.  
        /// </summary>
        /// <returns></returns>
        public List<ApplicationSubscription> GetAllSubscriptions()
        {
            List<ApplicationSubscription> subscriptions = new List<ApplicationSubscription>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("[MessageQueue].[GetAllSubscriptions]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var sqlReader = command.ExecuteReader())
                    {
                        int appNameOrdinal = sqlReader.GetOrdinal("ApplicationQueue");
                        int eventTypeOrdinal = sqlReader.GetOrdinal("EventType");
                        while (sqlReader.Read())
                        {
                            string appName = sqlReader.GetString(appNameOrdinal);
                            string eventType = sqlReader.GetString(eventTypeOrdinal);
                            ApplicationSubscription sub = new ApplicationSubscription() { ApplicationName = appName, EventType = Type.GetType(eventType) };
                            subscriptions.Add(sub);
                        }
                    }
                }
            }
            return subscriptions;
        }

        /// <summary>
        /// Serializes an event message to a string.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Serialize(EventMessage message)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    _serializer.WriteObject(xmlWriter, message);
                }
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Deserializes a string into an event message.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public EventMessage DeserializeEventMessage(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var xmlreader = XmlReader.Create(stringReader))
                {
                    return (EventMessage)_serializer.ReadObject(xmlreader);
                }
            }
        }

    }
}
