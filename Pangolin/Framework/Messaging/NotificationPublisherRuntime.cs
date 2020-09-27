using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Logging;
using System.Linq;
using EnderPi.Framework.Caching;


namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// This is the class that publishes and routes messages to subscribing applications.  It is separate from the service so that it can be unit tested.
    /// </summary>
    /// <remarks>
    /// This class pulls events off the global publishing queue and routes copies to every subscribing application.  It actually uses it's own framework in a monitored
    /// cache to listen for subscription changes.  The windows service that hosts this will more or less create an instance of this and call start().
    /// </remarks>
    public class NotificationPublisherRuntime
    {

        /// <summary>
        /// The queue cache just holds all the application event queues.  Could probably be statically cached, but I dislike static caches.
        /// </summary>
        private TimedCache _queueCache;


        /// <summary>
        /// The receiving, or global, event queue.
        /// </summary>
        private IMessageQueue _receivingQueue;

        /// <summary>
        /// The listener that does the bulk of the work, pulling messages from the event queue and routing them to other queues
        /// </summary>
        private ThrottledTaskProcessor<Message> _publicationListener;

        private IEventManager _eventManager;

        private Logger _logger;

        /// <summary>
        /// The subscription cache.  
        /// </summary>
        private MonitoredCache _subscriptionCache;

        private string _connectionString;


        public NotificationPublisherRuntime(string connectionString, IMessageQueue eventQueue, IEventManager eventManager, Logger logger, int maxConcurrency, int maxTaskLifetimeInSeconds, int millisecondsToSleep)
        {
            //Store some references
            _connectionString = connectionString;
            _receivingQueue = eventQueue;
            _eventManager = eventManager;
            _logger = logger;

            //Creating some simple objects
            _subscriptionCache = new MonitoredCache(CacheNames.EventSubscriptions, _eventManager);
            _queueCache = new TimedCache(60 * 60);

            //Setting up the publication listener
            _publicationListener = new ThrottledTaskProcessor<Message>(GetEvent, PublishNotification);
            _publicationListener.MaximumWorkerThreads = maxConcurrency;
            _publicationListener.MaxTaskLifeTimeInSeconds = maxTaskLifetimeInSeconds;
            _publicationListener.MillisecondsToSleep = millisecondsToSleep;
            _publicationListener.ExceptionOccurredInWorkerThread += LogException;
            _publicationListener.ExceptionOccurredInSchedulerThread += LogException;     
            //TODO need to add event handlers for other events, to log debug and info stuff.
        }

        /// <summary>
        /// Starts the notification publisher.
        /// </summary>
        public void Start()
        {
            _publicationListener.StartListening();
        }

        /// <summary>
        /// Stops the notification publisher.
        /// </summary>
        public void Stop()
        {
            _publicationListener.StopListening();
        }

        /// <summary>
        /// Deserializes the message so it can see the event, then routes it to every subscribing application by sending
        /// a copy to the subscribing application's event queue.
        /// </summary>
        /// <param name="message"></param>
        private void PublishNotification(Message message)
        {
            //The task processor framework guarantees the message isn't null, we need to deserialize it just to see the type.
            //TODO since we don't use the event message, maybe we need to push this into the event manager, 
            //have an IEventManager.GetEventType(string)
            EventMessage eventMessage = _eventManager.DeserializeEventMessage(message.Body);
            Type eventType = eventMessage.GetType();

            var subscriptions = GetSubscriptionMappings().Where(y => y.EventType == eventType);
            var applications = subscriptions.Select(x => x.ApplicationName).Distinct();
            foreach (var application in applications)
            {
                Threading.Threading.ExecuteWithoutThrowing(() => SendMessageToQueue(application, message));
            }
        }

        /// <summary>
        /// Logs exceptions coming from the underlying throttled event processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogException(object sender, ThrottledEventArgs<Message> e)
        {
            if (e != null && e.Exception != null)
            {
                _logger.Log(e.Exception.ToString(), LoggingLevel.Error);
            }
        }

        private void SendMessageToQueue(string application, Message message)
        {
            var messageQueue = _queueCache.Fetch("QueueName_" + application, () => { return new MessageQueue(_connectionString, application); });
            messageQueue.SendMessage(message);
        }

        private List<ApplicationSubscription> GetSubscriptionMappings()
        {
            return _subscriptionCache.Fetch("Subscriptions", () => _eventManager.GetAllSubscriptions());
        }

        /// <summary>
        /// Trivial delegate to get the next event to publish.
        /// </summary>
        /// <returns></returns>
        private Message GetEvent()
        {
            return _receivingQueue.GetNextMessage();
        }                

    }
}
