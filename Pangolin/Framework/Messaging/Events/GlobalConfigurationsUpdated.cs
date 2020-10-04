using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging.Events
{
    /// <summary>
    /// This event indicates that the given key has been inserted, updated, or deleted.
    /// </summary>
    /// <remarks>
    /// This is part of an experiment in strongly typed cache invalidation events, that carry specific information, so that individual records may be invalidated.
    /// As a thought experiemtn, this is problematic for multiple inserts, updates, or deletes, as it will cause a flood of messages.  Probably not an issue for globals.
    /// I suppose the issue of cache invalidation needs to be handled appropriately by each strongly typed cache provider.
    /// If it's something like global settings, that can simply be dropped, and the entire table re-read.  Not a problem.
    /// If it's something much, much, much bigger, then maybe it is actually worthwhile to invalidate a single key in the dictionary.
    /// This feels like it's completely irrelevant at the current scale of my application.
    /// </remarks>
    [DataContract(Name = "GlobalConfigurationsUpdated", Namespace = "EnderPi")]
    public class GlobalConfigurationsUpdated : EventMessage
    {
        [DataMember(Name = "Key")]
        public string Key { set; get; }
    }
}
