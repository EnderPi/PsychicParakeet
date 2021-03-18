using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// This is a message queue backed by SQL server.  Simple API.
    /// </summary>
    public class MessageQueue : IMessageQueue
    {        
        private static readonly Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9]*$");

        private string _queueName;
        private string _databaseConnection;
        private const string _createTableStatement = @"IF (SELECT OBJECT_ID('MessageQueue.{0}')) IS NULL BEGIN CREATE TABLE MessageQueue.{0} (Id BIGINT IDENTITY(1,1), Priority int, DateCreated DateTime , MessageBody VARCHAR(MAX)) CREATE CLUSTERED INDEX [MessageIndex] ON MessageQueue.{0} ([Priority] DESC, [ID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) END";
        private const string _insertMessageStatement = @"INSERT INTO [MessageQueue].{0} ([Priority],[DateCreated],[MessageBody]) VALUES (@Priority,@DateCreated,@MessageBody)";
        private const string _readMessageStatement = @"WITH T AS (SELECT TOP 1 [Id],[Priority],[DateCreated],[MessageBody] FROM MessageQueue.{0} ORDER BY PRIORITY DESC, ID ASC) DELETE FROM T OUTPUT DELETED.ID, DELETED.Priority, DELETED.DateCreated, DELETED.MessageBody";        
        private const string _peekMessageStatement = "SELECT TOP 1 [Id],[Priority],[DateCreated],[MessageBody] FROM MessageQueue.{0} ORDER BY PRIORITY DESC, ID DESC";
        private const string _countMessagesStatement = "SELECT COUNT(*) FROM MessageQueue.{0}";
        private const string _clearMessagesStatement = "DELETE FROM MessageQueue.{0}";

        public string Name => _queueName;

        /// <summary>
        /// Constructs the object, builds the underlying table if it doesn't exist.
        /// </summary>
        /// <param name="Name">The name of the queue</param>
        public MessageQueue(string databaseConnection, string queueName)
        {
            if (!IsQueueNameValid(queueName))
            {
                throw new ArgumentException("Queue name is not valid.  Must consist of onlye letters and numbers, and must begin with a letter.");
            }
            _databaseConnection = databaseConnection;
            _queueName = queueName;
            CreateTable();            
        }

        private bool IsQueueNameValid(string queueName)
        {
            return regex.IsMatch(queueName);
        }

        /// <summary>
        /// Creates the table.  Safe to call if table already exists.
        /// </summary>
        private void CreateTable()
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_createTableStatement, _queueName), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
                        
        public void SendMessage(Message message)
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_insertMessageStatement, _queueName), connection))
                {
                    command.Parameters.Add("@Priority", System.Data.SqlDbType.Int).Value = (int)message.Priority;
                    command.Parameters.Add("@DateCreated", System.Data.SqlDbType.DateTime).Value = message.DateCreated;
                    command.Parameters.Add("@MessageBody", System.Data.SqlDbType.VarChar).Value = message.Body;
                    command.ExecuteNonQuery();
                }
            }            
        }

        /// <summary>
        /// Returns a message if one is on the queue, or null if there isn't one.
        /// </summary>
        /// <returns>The next message, or null if one doesn't exist.</returns>
        public Message GetNextMessage()
        {
            Message message = null;
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_readMessageStatement, _queueName), connection))
                {
                    using (var sqlReader = command.ExecuteReader())
                    {
                        if (sqlReader.Read())
                        {
                            long id = sqlReader.GetInt64(0);
                            MessagePriority priority = (MessagePriority)sqlReader.GetInt32(1);
                            DateTime timestamp = sqlReader.GetDateTime(2);
                            string body = sqlReader.GetString(3);
                            message = new Message(id, body, timestamp, priority);
                        }
                    }
                }
            }            
            return message;
        }

        /// <summary>
        /// Returns the next message without removing it from the queue.
        /// </summary>
        /// <returns>The next message, or null if it doesn't exist.</returns>
        public Message PeekMessage()
        {
            Message message = null;
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_peekMessageStatement, _queueName), connection))
                {
                    using (var sqlReader = command.ExecuteReader())
                    {
                        if (sqlReader.Read())
                        {
                            long id = sqlReader.GetInt64(0);
                            MessagePriority priority = (MessagePriority)sqlReader.GetInt32(1);
                            DateTime timestamp = sqlReader.GetDateTime(2);
                            string body = sqlReader.GetString(3);
                            message = new Message(id, body, timestamp, priority);
                        }
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// The number of messages in the queue.
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_countMessagesStatement, _queueName), connection))
                {
                    return (long)command.ExecuteScalar();                    
                }
            }            
        }        

        /// <summary>
        /// Clears the entire queue.  Use with caution.
        /// </summary>
        /// <returns></returns>
        public int Clear()
        {
            using (SqlConnection connection = new SqlConnection(_databaseConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(_clearMessagesStatement, _queueName), connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

    }
}
