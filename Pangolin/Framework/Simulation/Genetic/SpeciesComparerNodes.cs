using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class SpeciesComparerNodes : IComparer<RngSpecies>
    {
        public int Compare([AllowNull] RngSpecies x, [AllowNull] RngSpecies y)
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
