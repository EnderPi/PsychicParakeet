using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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

        public void WriteLogRecord(LogMessage logMessage, LogDetails details)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Logging].[CreateLogWithDetails]", sqlConnection))
                {
                    SqlParameter parameter;
                    parameter = command.Parameters.AddWithValue("@LogDetails", CreateDataTable(details));
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[Logging].[LogDetails]";
                    
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

        private static DataTable CreateDataTable(LogDetails details)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Key", typeof(string));
            table.Columns.Add("Value", typeof(string));
            if (details != null)
            {
                foreach (var detail in details.Values)
                {
                    table.Rows.Add(detail.Item1, detail.Item2);
                }
            }
            return table;
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

        public void WriteLogRecord(LogMessage logMessage)
        {
            WriteLogRecord(logMessage, null);
        }

        public LogDetails GetLogDetails(long Id)
        {
            LogDetails details = new LogDetails();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Logging].[GetLogDetails]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.BigInt).Value = Id;
                    sqlConnection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int keypos = reader.GetOrdinal("Key");
                        int valuePos = reader.GetOrdinal("Value");
                        while (reader.Read())
                        {
                            details.AddDetail(reader.GetString(keypos), reader.GetString(valuePos));
                        }
                    }
                }
            }
            return details;
        }

        public LogMessage[] SearchLogMessages(LogSearchModel searchModel)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(CreateSearchQuery(searchModel), sqlConnection))
                {
                    command.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    return ReadLogMessages(command).ToArray();
                }
            }
        }

        private string CreateSearchQuery(LogSearchModel model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP (25) [Id], [Source], [TimeStamp], [LogLevel], [Message] FROM [Logging].[Log] WITH (NOLOCK) ");

            string whereString = BuildWhereClaus(model);
            
            if (!string.IsNullOrWhiteSpace(whereString))
            {
                sb.Append("WHERE ");
                sb.Append(whereString);
            }
            sb.Append(" ORDER BY [Id] DESC");
            return sb.ToString();
        }

        private string BuildWhereClaus(LogSearchModel model)
        {
            string whereClause = null;
            List<string> whereParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(model.Source))
            {
                whereParts.Add($"[Source]='{model.Source}'");
            }
            if (model.BeginTime != DateTime.MinValue && model.EndTime != DateTime.MinValue)
            {
                whereParts.Add($"[TimeStamp] BETWEEN '{SqlHelper.ToSqlDateTimeString(model.BeginTime)}' AND '{SqlHelper.ToSqlDateTimeString(model.EndTime)}'");
            }
            if (!string.IsNullOrWhiteSpace(model.Message))
            {
                whereParts.Add($"[Message] LIKE '%{model.Message}%'");
            }
            LoggingLevel loggingLevel = GetLoggingLevel(model);
            whereParts.Add($"[LogLevel] & {(int)loggingLevel} != 0");
            if (whereParts.Count > 0)
            {
                whereClause = string.Join(" AND ", whereParts);
            }
            return whereClause;
        }

        private static LoggingLevel GetLoggingLevel(LogSearchModel model)
        {
            LoggingLevel loggingLevel = LoggingLevel.None;
            if (model.ShowDebug)
            {
                loggingLevel |= LoggingLevel.Debug;
            }
            if (model.ShowInformation)
            {
                loggingLevel |= LoggingLevel.Information;
            }
            if (model.ShowWarning)
            {
                loggingLevel |= LoggingLevel.Warning;
            }
            if (model.ShowError)
            {
                loggingLevel |= LoggingLevel.Error;
            }
            if (model.ShowFatal)
            {
                loggingLevel |= LoggingLevel.Fatal;
            }

            return loggingLevel;
        }
    }
}
