using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging.Events
{
    [DataContract(Name = "StopBackgroundTasksEvent", Namespace = "EnderPi")]
    public class StopBackgroundTasksEvent :EventMessage
    {
    }
}
