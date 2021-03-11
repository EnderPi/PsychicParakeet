using EnderPi.Framework.Simulation.RandomnessTest;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// A record in the BirthdayTest table.
    /// </summary>
    public class BirthdayTestPoco
    {
        public int SimulationId { set; get; }
        public int NumberOfIterations { set; get; }
        public int Lambda { set; get; }
        public double PValue { set; get; }
        public TestResult Result { set; get; }
        public string DetailedResult { set; get; }        
    }
}
