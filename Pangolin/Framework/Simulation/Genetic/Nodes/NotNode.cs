using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class NotNode : TreeNode
    {
        public NotNode(TreeNode node)
        {
            _children = new List<TreeNode>() { node };
        }

        public override double Cost()
        {
            return 2.1;
        }

        public override string Evaluate()
        {
            return $"(Not {_children[0].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"Not({_children[0].EvaluatePretty()})";
        }
    }
}
