using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;


namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// Generic data access for message queue-related things.
    /// </summary>
    public class MessageQueueDataAccess
    {
        private const string _deleteTableStatement = "IF (SELECT OBJECT_ID('MessageQueue.{0}')) IS NOT NULL BEGIN DROP TABLE MessageQueue.{0} END";
        private const string _countOfQueuesStatement = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'MessageQueue'";

        private string _connectionString;

        public MessageQueueDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Completely deletes the queue.  Safe to call on non-existent queues.
        /// </summary>
        /// <param name="queueName"></param>
        public void DeleteQueue(string queueName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_deleteTableStatement, queueName), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public int GetCountOfQueues()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(_countOfQueuesStatement, connection))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

    }
}
