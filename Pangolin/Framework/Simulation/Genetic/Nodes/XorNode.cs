using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class XorNode :TreeNode
    {
        public XorNode(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>(2) { left, right };
        }

        public override double Cost()
        {
            return 2.1;
        }

        public override string Evaluate()
        {
            return $"({_children[0].Evaluate()} Xor {_children[1].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"({_children[0].EvaluatePretty()} Xor {_children[1].EvaluatePretty()})";
        }
    }
}
