using NUnit.Framework;
using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Services;
using EnderPi.Framework.Logging;
using EnderPi.Framework.DataAccess;
using System.Threading;
using EnderPi.Framework.Simulation;
using EnderPi.Framework.Threading;

namespace UnitTest.Framework
{
    public class BackgroundWorkerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test for the background worker runtime.  SO it creates an instancew of the runtime, starts it,
        /// submits a background task, waits a second, then checks to see that the background task has finished.
        /// The background task does nothing other than log a message and write a message to a queue with the
        /// passed in parameter, which is concatenated with itself.  It writes to a queue that is injected via the service provider.        /// 
        /// </summary>
        [Test]
        public void TestBackgroundWorker()
        {
            
            const string messageQueueName = "TestBackgroundWorkerQueue";
            try
            {
                MessageQueue queue = new MessageQueue(Globals.ConnectionString, messageQueueName);
                //create a background worker runtime and start it
                var logDataAccess = new LogDataAccess(Globals.ConnectionString);
                var configurationDataAccess = new ConfigurationDataAccess(Globals.ConnectionString);
                var backgroundLogger = new Logger(logDataAccess, "BackgroundWorker", LoggingLevel.Debug | LoggingLevel.Information | LoggingLevel.Warning | LoggingLevel.Error);
                var simulationDataAccess = new SimulationDataAccess(Globals.ConnectionString);
                var backgroundTaskManager = new BackgroundTaskManager(simulationDataAccess, backgroundLogger, configurationDataAccess);
                ServiceProvider serviceProvider = new ServiceProvider();
                serviceProvider.RegisterService<IMessageQueue>(queue);
                serviceProvider.RegisterService(backgroundLogger);

                var eventQueueNameSub = "GlobalTestEventQueue";    //The GLOBAL event queue
                IMessageQueue eventMessageQueueSub = new MessageQueue(Globals.ConnectionString, eventQueueNameSub);
                var notificationSubscriberApplicationEventQueueName = "NotificationSubscriberEventQueueTest";  //The event queue for the Subscriber App
                IMessageQueue subAppEventQueue = new MessageQueue(Globals.ConnectionString, notificationSubscriberApplicationEventQueueName);
                var taskParameters = new ThrottledTaskProcessorParameters(1, 30, 100, 120, false);
                EventManager subAppEventManager = new EventManager(Globals.ConnectionString, eventMessageQueueSub, subAppEventQueue, taskParameters, backgroundLogger);



                var backgroundWorkerRuntime = new BackgroundWorkerRuntime(backgroundTaskManager, taskParameters, serviceProvider, backgroundLogger, configurationDataAccess, subAppEventManager);

                backgroundWorkerRuntime.Start();

                //create a simulation and submit it
                TestSimulation simulation = new TestSimulation();
                backgroundTaskManager.SubmitBackgroundTask(simulation);

                //sleep for a while.
                Thread.Sleep(1000);

                //Verify the task ran, somehow
                var message1 = queue.GetNextMessage();
                var message2 = queue.GetNextMessage();
                var message3 = queue.GetNextMessage();
                Assert.IsTrue(message1.Body == "INITIALIZE");
                Assert.IsTrue(message2.Body == "START");
                Assert.IsTrue(message3.Body == "STORE");

                //clean up
                backgroundWorkerRuntime.Stop();
            }
            finally
            {
                MessageQueueDataAccess messageQueueDataAccess = new MessageQueueDataAccess(Globals.ConnectionString);
                messageQueueDataAccess.DeleteQueue(messageQueueName);
            }
        }

    }
}
