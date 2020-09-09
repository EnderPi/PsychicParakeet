using NUnit.Framework;
using System;
using EnderPi.Framework.Logging;
using EnderPi.Framework.DataAccess;
using System.Data.SqlClient;
using System.Data;

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
            string logSource = "Cow";
            Logger logger = new Logger(logDataAccess, logSource, LoggingLevel.Debug | LoggingLevel.Error);
            string testData = Guid.NewGuid().ToString();
            logger.Log(testData, logLevel);
            var exists = FindLogMessage(Globals.ConnectionString, logSource, testData, logLevel);
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




    }
}
