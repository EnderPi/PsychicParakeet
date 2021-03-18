using System;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class StateOneNode : TreeNode
    {
        public const string Name = "A";
        public StateOneNode() 
        {
        }

        public override double Cost()
        {
            return 0;
        }

        public override string Evaluate()
        {
            return Name;
        }

        public override string EvaluatePretty()
        {
            return Name;
        }
    }
}
