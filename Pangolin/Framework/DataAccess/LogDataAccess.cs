using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Logging;
using System.Data;
using System.Data.SqlClient;

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
    }
}
