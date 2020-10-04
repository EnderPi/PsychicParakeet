using EnderPi.Framework.Logging;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// POCO for application configuration settings for the notification publisher.
    /// </summary>
    public class NotificationPublisherAppSettings
    {
        /// <summary>
        /// Maximum number of threads.
        /// </summary>
        public int MaxConcurrency { set; get; }

        /// <summary>
        /// The event queue that the Notification publisher itself uses.
        /// </summary>
        public string EventQueueName { set; get; }

        public LoggingLevel ApplicationLoggingLevel { set; get; }

        public int MaxTaskLifeTimeInSeconds { set; get; }

        public int MillisecondsToSleep { set; get; }
    }
}
