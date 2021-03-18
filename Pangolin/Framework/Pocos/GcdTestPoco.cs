using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// Data transfer object
    /// </summary>
    public class GcdTestPoco
    {
        public int SimulationId { set; get; }
        public long NumberOfGcds { set; get; }
        public double PValueGcd { set; get; }
        public TestResult TestResultGcd { set; get; }
        public double PValueSTeps { set; get; }
        public TestResult TestResultSteps { set; get; }
        public string DetailedResult { set; get; }        
    }
}
