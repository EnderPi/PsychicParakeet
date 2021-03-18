using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class ConstantNode : TreeNode
    {
        private ulong _state;

        public string Name { set; get; }

        public ulong Value { get { return _state; } set { _state = value; } }

        public ConstantNode(ulong constant)
        {
            _state = constant;
            _children = new List<TreeNode>();
        }

        public override double Cost()
        {
            return 0;
        }

        public override string Evaluate()
        {
            return $"{_state}LU";
        }

        public override string EvaluatePretty()
        {
            return Name;
        }
    }
}
