using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic.Nodes32Bit
{
    [Serializable]
    public class StateNode32 :TreeNode
    {
        public const string Name = "X";
        public StateNode32()
        {
        }

        public override double Cost()
        {
            return 0;
        }

        public override string Evaluate()
        {
            return Name;
        }

        public override string EvaluatePretty()
        {
            return Name;
        }
    }
}
