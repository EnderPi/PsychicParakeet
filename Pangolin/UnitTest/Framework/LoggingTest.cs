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
        //"Server=(localdb)\\mssqllocaldb;Database=aspnet-LogViewer-E2A8655F-A68B-4562-AE75-7EE36E643F03;Trusted_Connection=True;MultipleActiveResultSets=true"
        private string _connectionString = "Server=localhost;Integrated Security = SSPI; Database=PangolinDev;Trusted_Connection=True;";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("Cow", LoggingLevel.Debug, true)]
        [TestCase("Cow", LoggingLevel.Information, false)]
        public void TestLogger(string source, LoggingLevel logLevel, bool Result)
        {
            LogDataAccess logDataAccess = new LogDataAccess(_connectionString);
            string logSource = "Cow";
            Logger logger = new Logger(logDataAccess, logSource, LoggingLevel.Debug | LoggingLevel.Error);
            string testData = Guid.NewGuid().ToString();
            logger.Log(testData, logLevel);
            var exists = FindLogMessage(_connectionString, logSource, testData, logLevel);
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
    }
}
