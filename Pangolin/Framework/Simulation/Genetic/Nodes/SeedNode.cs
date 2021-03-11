using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class SeedNode : TreeNode
    {
        public const string Name = "Q";
        public SeedNode()
        {
            _children = new List<TreeNode>();
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
