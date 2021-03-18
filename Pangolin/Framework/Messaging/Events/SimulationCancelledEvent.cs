using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging.Events
{
    /// <summary>
    /// Occurs when a user requests a simulation be cancelled.
    /// </summary>
    [DataContract(Name = "SimulationCancelledEvent", Namespace = "EnderPi")]
    public class SimulationCancelledEvent : EventMessage
    {
        public int SimulationId { set; get; }
    }
}
