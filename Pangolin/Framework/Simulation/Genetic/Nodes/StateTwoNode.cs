using System;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class StateTwoNode :TreeNode
    {
        public const string Name = "B";
        public StateTwoNode() { }

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
