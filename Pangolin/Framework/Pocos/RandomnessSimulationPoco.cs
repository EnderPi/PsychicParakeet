using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// POCO for data access.
    /// </summary>
    public class RandomnessSimulationPoco
    {
        public int SimulationId { set; get; }
        public long NumbersGenerated { set; get; }
        public long TargetNumbersGenerated { set; get; }
        public TestResult Result { set; get; }
        public string RandomNumberEngine { set; get; }
        public string Description { set; get; }
    }
}
