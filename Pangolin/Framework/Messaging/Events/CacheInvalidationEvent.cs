using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging.Events
{
    [DataContract(Name = "CacheInvalidationEvent", Namespace = "EnderPi")]
    public class CacheInvalidationEvent : EventMessage
    {
        /// <summary>
        /// The name of the cache that was invalidated.
        /// </summary>
        [DataMember(Name = "CacheName")]
        public string CacheName { set; get; }

        /// <summary>
        /// Not necessarily used all the time, but if you want to signal that only a single row was updated, you could do this.
        /// </summary>
        [DataMember(Name = "UpdatedKey")]
        public string UpdatedKey { set; get; }
    }
}
