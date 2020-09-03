using EnderPi.Framework.Caching;
using EnderPi.Framework.Messaging;
using NUnit.Framework;
using System;

namespace UnitTest.Framework
{
    public class MessageQueueTest
    {
        private string _connectionString = "Server=localhost;Integrated Security = SSPI; Database=PangolinDev;Trusted_Connection=True;";


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMessageQueue()
        {
            TimedCache cache = new TimedCache();
            string queueName = "TestQueue";
            MessageQueue myQueue = new MessageQueue(_connectionString, queueName, cache);
            string messageBodyHigh = "Test Message Body";
            Message messageHigh = new Message(0, messageBodyHigh, DateTime.Now, MessagePriority.High);
            myQueue.SendMessage(messageHigh);
            string messageBodylow = "Test Message Body Low";
            Message messageLow = new Message(0, messageBodylow, DateTime.Now, MessagePriority.Low);
            myQueue.SendMessage(messageLow);
            var message1 = myQueue.GetNextMessage();
            Assert.IsTrue(string.Equals(message1.Body, messageBodyHigh));
            var message2 = myQueue.GetNextMessage();
            Assert.IsTrue(string.Equals(message2.Body, messageBodylow));
            MessageQueueDataAccess dataAccess = new MessageQueueDataAccess(_connectionString);
            dataAccess.DeleteQueue(queueName);
        }

    }
}
