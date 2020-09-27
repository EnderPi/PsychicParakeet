using EnderPi.Framework.Caching;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Messaging;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace UnitTest.Framework
{
    public class MessageQueueTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Creates a queue, pushes in a high and low priority message, pulls out the messages, and deletes the queue.
        /// Verifies they come out in the write order.
        /// </summary>
        [Test]
        public void TestMessageQueue()
        {
            string queueName = "TestQueue";
            MessageQueue myQueue = new MessageQueue(Globals.ConnectionString, queueName);
            string messageBodyHigh = "Test Message Body";
            Message messageHigh = new Message(0, messageBodyHigh, DateTime.Now, MessagePriority.High);
            string messageBodylow = "Test Message Body Low";
            Message messageLow = new Message(0, messageBodylow, DateTime.Now, MessagePriority.Low);
            myQueue.SendMessage(messageLow);
            myQueue.SendMessage(messageHigh);
            var message1 = myQueue.GetNextMessage();
            Assert.IsTrue(string.Equals(message1.Body, messageBodyHigh));
            var message2 = myQueue.GetNextMessage();
            Assert.IsTrue(string.Equals(message2.Body, messageBodylow));
            MessageQueueDataAccess dataAccess = new MessageQueueDataAccess(Globals.ConnectionString);
            dataAccess.DeleteQueue(queueName);
        }

        /// <summary>
        /// Performed 1000 null reads in 127 milliseconds, so it's ~127 microseconds to do a null read.
        /// </summary>
        [Test]
        public void MessageQueueEmptySpeed()
        {
            LogDataAccess logDataAccess = new LogDataAccess(Globals.ConnectionString);
            Logger logger = new Logger(logDataAccess, "MessageQueue", LoggingLevel.Debug);
            MessageQueue queue = new MessageQueue(Globals.ConnectionString, "TestMySpeedQueue");
            Message message;
            Stopwatch watch = Stopwatch.StartNew();
            for (int i=0; i< 1000; i++)
            {
                message = queue.GetNextMessage();
            }
            watch.Stop();
            logger.Log($"Read 1000 null messages in {watch.ElapsedMilliseconds} milliseconds.", LoggingLevel.Debug);
        }


    }
}
