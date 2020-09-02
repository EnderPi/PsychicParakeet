using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// POCO message for use with the messaging service
    /// </summary>
    public class Message
    {
        public Message(long identity, string body, DateTime created, MessagePriority priority)
        {
            Priority = priority;
            Body = body;
            DateCreated = created;
            Identity = identity;
        }
        
        /// <summary>
        /// The priority of the message.
        /// </summary>
        public MessagePriority Priority { get; set; }

        /// <summary>
        /// This is a poco, so this is the identity in the table.
        /// </summary>
        public long Identity { get; set; }

        /// <summary>
        /// The message body, probably something xml serialized.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The date the message was created.  
        /// </summary>
        public DateTime DateCreated { get; set; }
                        
    }
}
