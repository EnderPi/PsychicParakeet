using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic.Nodes32Bit
{
    [Serializable]
    public class RindjaelNode32 :TreeNode
    {
        public RindjaelNode32(TreeNode child)
        {
            _children = new List<TreeNode>() { child };
        }

        public override double Cost()
        {
            return 60;
        }

        public override string Evaluate()
        {
            return $"Rindjael32({_children[0].Evaluate()})";
        }

        public override string EvaluatePretty()
        {
            return $"Rindjael32({_children[0].EvaluatePretty()})";
        }
    }
}
