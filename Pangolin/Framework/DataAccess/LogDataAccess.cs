using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Data access class for the logs in the daterbase.
    /// </summary>
    public class LogDataAccess : ILogDataAccess
    {
        private string _connectionString;

        public LogDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void WriteLogRecord(LogMessage logMessage)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Logging].[CreateLog]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Source", SqlDbType.VarChar, 100).Value = logMessage.Source;
                    command.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = logMessage.TimeStamp;
                    command.Parameters.Add("@LogLevel", SqlDbType.TinyInt).Value = (byte)logMessage.LogLevel;
                    command.Parameters.Add("@Message", SqlDbType.VarChar, -1).Value = logMessage.Message;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }                
        }

        public int GetCurrentCount()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Logging].[GetLogCount]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public LogMessage[] GetTopLogMessages(int count)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Logging].[GetRecentLogs]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Count", SqlDbType.Int).Value = count;
                    sqlConnection.Open();
                    return ReadLogMessages(command).ToArray();
                }
            }
        }

        public IEnumerable<LogMessage> ReadLogMessages(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int sourcePos = reader.GetOrdinal("Source");
                int timeStampPos = reader.GetOrdinal("TimeStamp");
                int messagePos = reader.GetOrdinal("Message");
                int logLevelPos = reader.GetOrdinal("LogLevel");
                while (reader.Read())
                {
                    long id = reader.GetInt64(idPos);
                    string source = reader.GetString(sourcePos);
                    DateTime timeStamp = reader.GetDateTime(timeStampPos);
                    string message = reader.GetString(messagePos);
                    LoggingLevel logLevel = (LoggingLevel)reader.GetByte(logLevelPos);
                    yield return new LogMessage(id, source, timeStamp, logLevel, message);
                }
            }
        }

    }
}
