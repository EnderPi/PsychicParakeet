using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic.Nodes32Bit
{
    [Serializable]
    public class ConstantNode32bit :TreeNode
    {
        private uint _state;

        public string Name { set; get; }

        public uint Value { get { return _state; } set { _state = value; } }

        public ConstantNode32bit(uint constant)
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
            return $"{_state}U";
        }

        public override string EvaluatePretty()
        {
            return Name;
        }
    }
}
