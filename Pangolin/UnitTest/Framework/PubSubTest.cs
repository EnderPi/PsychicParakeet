using System.Threading;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Messaging.Events;
using NUnit.Framework;
using EnderPi.Framework.Threading;

namespace UnitTest.Framework
{
    public class PubSubTest
    {
        private int _counter;


        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// This tests the pub sub framework.  Create all the framework of a publisher runtime and a separate event manager that simulates a separate process.
        /// Subscribe to an event, publish the event a couple times, confirm the delegate was called, unsubscribe, confirm the delegate was not called.
        /// Speeds up the process by dramatically reducing sleep times in the classes it builds.
        /// Incidentally tests subscription, unsubscription, publishing, and the monitored cache (which is used by the publisher itself to invalidate subscriptions)
        /// </summary>
        [Test]
        public void TestPubSub()
        {
            _counter = 0;

            //create pubsub runtime, and all state for pub sub app
            var logDataAccess = new LogDataAccess(Globals.ConnectionString);
            var notificationLogger = new Logger(logDataAccess, "NotificationPublisher", LoggingLevel.Debug | LoggingLevel.Information | LoggingLevel.Warning | LoggingLevel.Error);
            var eventQueueName = "GlobalTestEventQueue";    //The GLOBAL event queue
            IMessageQueue eventMessageQueue = new MessageQueue(Globals.ConnectionString, eventQueueName);
            var notificationPublisherApplicationEventQueueName = "NotificationAppEventQueueTest";  //The event queue for the Notification App
            IMessageQueue notificationAppEventQueue = new MessageQueue(Globals.ConnectionString, notificationPublisherApplicationEventQueueName);
            var taskParameters = new ThrottledTaskProcessorParameters(1, 30, 100, 120, false);
            EventManager notificationAppEventManager = new EventManager(Globals.ConnectionString, eventMessageQueue, notificationAppEventQueue, taskParameters, notificationLogger);
            var notificationPublisher = new NotificationPublisherRuntime(Globals.ConnectionString, eventMessageQueue, notificationAppEventManager, notificationLogger, 1, 30, 100);

            //create subscribing application eventmanager
            var subLogger = new Logger(logDataAccess, "NotificationSubscriber", LoggingLevel.Error);
            var eventQueueNameSub = "GlobalTestEventQueue";    //The GLOBAL event queue
            IMessageQueue eventMessageQueueSub = new MessageQueue(Globals.ConnectionString, eventQueueNameSub);
            var notificationSubscriberApplicationEventQueueName = "NotificationSubscriberEventQueueTest";  //The event queue for the Subscriber App
            IMessageQueue subAppEventQueue = new MessageQueue(Globals.ConnectionString, notificationSubscriberApplicationEventQueueName);
            EventManager subAppEventManager = new EventManager(Globals.ConnectionString, eventMessageQueueSub, subAppEventQueue, taskParameters, subLogger);

            //start things
            notificationAppEventManager.StartListening();
            notificationPublisher.Start();
            subAppEventManager.StartListening();

            //subscribe a delegate to an event
            subAppEventManager.Subscribe<CacheInvalidationEvent>(ProcessSubscription);

            //wait (the publisher may take several seconds to invalidate it's own subscription cache)
            Thread.Sleep(1000);

            //publish an event (from the notification app, should probably be a third app)
            CacheInvalidationEvent e = new CacheInvalidationEvent() { CacheName = "COW" };
            notificationAppEventManager.PublishEvent(e);
            notificationAppEventManager.PublishEvent(e);
            Thread.Sleep(1000);

            //assert delegate was called
            Assert.AreEqual(2, _counter);

            //unsubscribe
            subAppEventManager.Unsubscribe<CacheInvalidationEvent>(ProcessSubscription);            

            //publish event
            CacheInvalidationEvent e3 = new CacheInvalidationEvent() { CacheName = "COW" };
            notificationAppEventManager.PublishEvent(e);
            Thread.Sleep(1000);

            //assert event delegate was not called.
            Assert.AreEqual(2, _counter);

            //that'll do.
            //cleanup
            subAppEventManager.StopListening();
            notificationAppEventManager.StopListening();
            notificationPublisher.Stop();

            var messageQueueDataAccess = new MessageQueueDataAccess(Globals.ConnectionString);
            messageQueueDataAccess.DeleteQueue(eventQueueName);
            messageQueueDataAccess.DeleteQueue(eventQueueNameSub);
            messageQueueDataAccess.DeleteQueue(notificationPublisherApplicationEventQueueName);
            messageQueueDataAccess.DeleteQueue(notificationSubscriberApplicationEventQueueName);
        }


        public void ProcessSubscription(CacheInvalidationEvent e)
        {
            if (e.CacheName == "COW")
            {
                _counter++;
            }
        }


    }
}
