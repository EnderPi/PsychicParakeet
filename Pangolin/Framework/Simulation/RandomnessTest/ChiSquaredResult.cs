using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class ChiSquaredResult
    {
        public double PValue { set; get; }

        public TestResult Result { set; get; }

        public List<ChiSquaredDetail> TopContributors { set; get; }
    }
}
