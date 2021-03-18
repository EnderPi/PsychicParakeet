using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class AndNode : TreeNode
    {
        public AndNode(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>() { left, right };
        }

        public override double Cost()
        {
            return 2.1;//estimate
        }

        public override string Evaluate()
        {
            return $"({_children[0].Evaluate()} And {_children[1].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"({_children[0].EvaluatePretty()} And {_children[1].EvaluatePretty()})";
        }
    }
}
