using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Logging
{
    /// <summary>
    /// POCO for use with the logging data access class.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// The Id of the message in the database.
        /// </summary>
        public long Id { set; get; }

        /// <summary>
        /// The source, which is typically the application name.
        /// </summary>
        public string Source { set; get; }

        /// <summary>
        /// The time when this message was created.
        /// </summary>
        public DateTime TimeStamp { set; get; }

        /// <summary>
        /// The LogLevel of the message
        /// </summary>
        public LoggingLevel LogLevel { set; get; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { set; get; }

        /// <summary>
        /// Plain constructor for the POCO.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="timeStamp"></param>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        public LogMessage(long id, string source, DateTime timeStamp, LoggingLevel logLevel, string message)
        {
            Id = id;
            Source = source;
            TimeStamp = timeStamp;
            LogLevel = logLevel;
            Message = message;
        }
    }
}
