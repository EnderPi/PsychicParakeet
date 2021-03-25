using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic.Nodes
{
    /// <summary>
    /// Rindjael S-box - see what the code does when given a simple non-linear operation like this.
    /// </summary>
    [Serializable]
    public class RindjaelNode : TreeNode
    {
        public RindjaelNode(TreeNode child)
        {
            _children = new List<TreeNode>() { child };
        }

        public override double Cost()
        {
            return 60;
        }

        public override string Evaluate()
        {
            return $"Rindjael({_children[0].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"Rindjael({_children[0].EvaluatePretty()})";
        }
    }
}
