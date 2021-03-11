using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class LeftShiftNode :TreeNode
    {
        public LeftShiftNode(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>(2) { left, right };
        }

        public override double Cost()
        {
            return 5.1;
        }

        public override string Evaluate()
        {
            return $"({_children[0].Evaluate()} << cast({_children[1].Evaluate()} And 63LU, int))";
        }

        public override string EvaluatePretty()
        {
            return $"LeftShift({_children[0].EvaluatePretty()}, {_children[1].EvaluatePretty()})";
        }
    }
}
