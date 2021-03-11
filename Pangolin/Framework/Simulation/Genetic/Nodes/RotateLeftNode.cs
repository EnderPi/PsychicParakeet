using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class RotateLeftNode :TreeNode
    {
        public RotateLeftNode(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>(2) { left, right };
        }

        public override double Cost()
        {
            return 5.1 + 5.1 + 2.1 + (_children[1] is ConstantNode ? 0 : (2.1 + 2.1 + 2.1));
        }

        public override string Evaluate()
        {
            return $"(   ({_children[0].Evaluate()}  <<  cast({_children[1].Evaluate()} And 63LU, int)) Or (  {_children[0].Evaluate()} >> (64-cast({_children[1].Evaluate()} And 63LU, int) ) ) )";
        }

        public override string EvaluatePretty()
        {
            return $"RotateLeft({_children[0].EvaluatePretty()}, {_children[1].EvaluatePretty()})";
        }
    }
}
