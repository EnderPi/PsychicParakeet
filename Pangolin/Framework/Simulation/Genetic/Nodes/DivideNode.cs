using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class DivideNode : TreeNode
    {
        public DivideNode(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>(2) { left, right };
        }

        public override double Cost()
        {
            return 50.1;
        }

        public override string Evaluate()
        {
            return $"({_children[0].Evaluate()}/{_children[1].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"({_children[0].EvaluatePretty()} / {_children[1].EvaluatePretty()})";
        }
    }
}
