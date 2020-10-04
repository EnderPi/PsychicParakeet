using System;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// This is used to map events to processors on the consuming application - in effect a local router.  Stored as process-local state in the eventmanager.
    /// </summary>
    /// <remarks>
    /// In the application, these will come either from coded cache monitor instances, or loaded from configuration tables.  
    /// Haven't worked out all the latter use cases yet.
    /// </remarks>
    public class EventSubscription
    {
        /// <summary>
        /// Probably not bad to persist to a DB.
        /// </summary>
        public Type EventType { set; get; }
        /// <summary>
        /// This doesn't get persisted across boundaries, although it may be created at run-time from reflection or the like.
        /// </summary>
        public Action<EventMessage> EventProcessor { set; get; }

        public object TargetMethod { set; get; }

    }
}
