using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// A record in the GeneticRng.GeneticSimulation table.
    /// </summary>
    public class GeneticSimulationPoco
    {
        public int Id { set; get; }

        public TestLevel Level { set; get; }

        public ConstraintMode ModeStateOne { set; get; }

        public ConstraintMode ModeStateTwo { set; get; }

        public GeneticCostMode CostMode { set; get; }

        public bool UseStateTwo { set; get; }

        public bool AllowAdditionNodes { set; get; }

        public bool AllowSubtractionNodes { set; get; }

        public bool AllowMultiplicationNodes { set; get; }

        public bool AllowDivisionNodes { set; get; }

        public bool AllowRemainderNodes { set; get; }

        public bool AllowRightShiftNodes { set; get; }

        public bool AllowLeftShiftNodes { set; get; }

        public bool AllowRotateLeftNodes { set; get; }

        public bool AllowRotateRightNodes { set; get; }

        public bool AllowAndNodes { set; get; }

        public bool AllowOrNodes { set; get; }

        public bool AllowNotNodes { set; get; }

        public bool AllowXorNodes { set; get; }
    }
}
