using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic.Nodes32Bit
{
    [Serializable]
    public class Rightshift32 :TreeNode
    {
        public Rightshift32(TreeNode left, TreeNode right)
        {
            _children = new List<TreeNode>(2) { left, right };
        }

        public override double Cost()
        {
            return 5.1;
        }

        public override string Evaluate()
        {
            return $"({_children[0].Evaluate()} >> cast({_children[1].Evaluate()} And 31U, int))";
        }

        public override string EvaluatePretty()
        {
            if (_children[1] is ConstantNode32bit constantNode)
            {
                return $"RightShift({_children[0].EvaluatePretty()}, {constantNode.Value & 31})";
            }
            else
            {
                return $"RightShift({_children[0].EvaluatePretty()}, {_children[1].EvaluatePretty()})";
            }
        }
    }
}
