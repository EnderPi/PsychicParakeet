using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class SeedRootNode :TreeNode
    {
        public SeedRootNode(TreeNode node)
        {
            _children = new List<TreeNode>() { node };
        }

        public override double Cost()
        {
            return 0;
        }

        public override string Evaluate()
        {
            return $"{_children[0].Evaluate()}";
        }

        public override string EvaluatePretty()
        {
            return $"{_children[0].EvaluatePretty()}";
        }
    }
}
