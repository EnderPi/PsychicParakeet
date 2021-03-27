using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.ComponentModel.DataAnnotations;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class GeneticParameters
    {
        public TestLevel Level { set; get; }

        public ConstraintMode ModeStateOne { set; get; }

        public ConstraintMode ModeStateTwo { set; get; }

        public GeneticCostMode CostMode { set; get; }

        public bool UseStateTwo { set; get; }

        public int Iterations { set; get; }

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
