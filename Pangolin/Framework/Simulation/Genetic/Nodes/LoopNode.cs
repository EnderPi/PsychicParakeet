using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic.Nodes
{
    /// <summary>
    /// Work in progress, implementation unclear.
    /// </summary>
    [Serializable]
    public class LoopNode : TreeNode
    {
        private int _loopCount;
        public LoopNode(TreeNode left, int counter)
        {
            _children = new List<TreeNode>() { left };
            _loopCount = counter % 9;
        }

        public override double Cost()
        {
            return 100;
        }

        public override string Evaluate()
        {
            string s = EvaluateInternal(_loopCount);
            return s;
        }

        public override string EvaluatePretty()
        {
            throw new NotImplementedException();
        }

        private string EvaluateInternal(int v)
        {
            if (--v <= 0)
            {
                return _children[0].Evaluate();
            }
            else
            {
                return _children[0].Evaluate().Replace("a", EvaluateInternal(v));
            }
        }
    }
}
