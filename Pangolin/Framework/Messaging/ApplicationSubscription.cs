using System;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// A subscription at the application level, which is just an application/queue name and an event type.
    /// </summary>
    /// <remarks>
    /// This is a POCO that is a row in a daterbase table.
    /// </remarks>
    public class ApplicationSubscription
    {
        /// <summary>
        /// The application name, which is the name of the table in the message queue schema.
        /// </summary>
        public string ApplicationName { set; get; }
        /// <summary>
        /// The event type of the subscription.
        /// </summary>
        public Type EventType { set; get; }
    }
}
