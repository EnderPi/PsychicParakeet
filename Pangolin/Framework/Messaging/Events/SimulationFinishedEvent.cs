using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace EnderPi.Framework.Messaging.Events
{
    [DataContract(Name = "SimulationFinishedEvent", Namespace = "EnderPi")]
    public class SimulationFinishedEvent : EventMessage
    {
        [DataMember(Name = "UserId")]
        public string UserName { set; get; }

        [DataMember(Name = "SimulationId")]
        public int SimulationId { set; get; }
    }
}
