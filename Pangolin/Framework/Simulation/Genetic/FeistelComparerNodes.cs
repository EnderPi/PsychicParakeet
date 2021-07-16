using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class FeistelComparerNodes : IComparer<RngSpecies32Feistel>
    {
        public int Compare([AllowNull] RngSpecies32Feistel x, [AllowNull] RngSpecies32Feistel y)
        {
            if (x.Fitness != y.Fitness)
            {
                return x.Fitness.CompareTo(y.Fitness);
            }
            else
            {
                if (x.TestsPassed == y.TestsPassed)
                {
                    return y.NodeCount.CompareTo(x.NodeCount);
                }
                else
                {
                    return x.TestsPassed.CompareTo(y.TestsPassed);
                }
            }
        }
    }
}
