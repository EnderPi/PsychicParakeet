﻿using NUnit.Framework;
using System;
using EnderPi.Framework.Logging;
using EnderPi.Framework.DataAccess;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace UnitTest.Framework
{
    public class LoggingTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("Cow", LoggingLevel.Debug, true)]
        [TestCase("Cow", LoggingLevel.Information, false)]
        public void TestLogger(string source, LoggingLevel logLevel, bool Result)
        {
            LogDataAccess logDataAccess = new LogDataAccess(Globals.ConnectionString);
            Logger logger = new Logger(logDataAccess, source, LoggingLevel.Debug | LoggingLevel.Error);
            string testData = Guid.NewGuid().ToString();
            logger.Log(testData, logLevel);
            var exists = FindLogMessage(Globals.ConnectionString, source, testData, logLevel);
            Assert.IsTrue(exists == Result);            
        }

        private bool FindLogMessage(string connectionString, string logSource, string testData, LoggingLevel debug)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                string commandText = $"SELECT 1 FROM [Logging].[Log] WHERE [Source]='{logSource}' AND [Message]='{testData}'";
                using (var command = new SqlCommand(commandText, sqlConnection))
                {
                    command.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    var rows =  command.ExecuteScalar() as int?;
                    return rows.HasValue && rows.Value == 1;
                }
            }            
        }

        [Test]
        public void TestLoggerWithDetails()
        {
            LogDataAccess logDataAccess = new LogDataAccess(Globals.ConnectionString);
            string logSource = "Cow";
            Logger logger = new Logger(logDataAccess, logSource, LoggingLevel.Debug | LoggingLevel.Error);
            string testData = Guid.NewGuid().ToString();
            LogDetails details = new LogDetails();
            details.AddDetail("order ID", "123456");
            details.AddDetail("User", "Tim the Enchanter");
            details.AddDetail("Customer ID", "123456");
            logger.Log(testData, LoggingLevel.Debug, details);
            var exists = FindLogMessage(Globals.ConnectionString, logSource, testData, LoggingLevel.Debug);
            Assert.IsTrue(exists);
        }

        /// <summary>
        /// Simple speed test.  Looks like 1.5 milliseconds to log a message with details.
        /// </summary>
        [Test]
        public void TestLoggerSpeedWithDetails()
        {
            LogDataAccess logDataAccess = new LogDataAccess(Globals.ConnectionString);
            string logSource = "Cow";
            Logger logger = new Logger(logDataAccess, logSource, LoggingLevel.Debug | LoggingLevel.Error);

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                string testData = Guid.NewGuid().ToString();
                LogDetails details = new LogDetails();
                details.AddDetail("order ID", "123456");
                details.AddDetail("User", "Tim the Enchanter");
                details.AddDetail("Customer ID", "123456");
                logger.Log(testData, LoggingLevel.Debug, details);
            }
            watch.Stop();

            logger.Log($"Logged 1000 messages with details in {watch.ElapsedMilliseconds} milliseconds.", LoggingLevel.Debug);

        }



    }
}
