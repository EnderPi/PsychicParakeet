using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging
{
    /// <summary>
    /// A message signifying an event occurred.  Derive from this class to create specific types.
    /// </summary>
    /// <remarks>
    /// If you derive from this type, you need to add the derived type to the known types, both here and in the eventManager class.
    /// </remarks>
    [DataContract(Name = "EventMessage", Namespace = "EnderPi")]
    [KnownType(typeof(Events.CacheInvalidationEvent))]
    [KnownType(typeof(Events.GlobalConfigurationsUpdated))]
    public abstract class EventMessage
    {

    }
}
