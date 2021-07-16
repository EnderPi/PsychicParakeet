using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnderPi.Framework.Simulation.Genetic
{
    internal class SpeciesComparerNodesAvalanche : IComparer<RngSpecies>
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
                    var xAvalanche = AvalancheScore(x);
                    var yAvalanche = AvalancheScore(y);
                    if (xAvalanche == yAvalanche)
                    {
                        return y.NodeCount.CompareTo(x.NodeCount);
                    }
                    else
                    {
                        return yAvalanche.CompareTo(xAvalanche);
                    }    
                }
                else
                {
                    return x.TestsPassed.CompareTo(y.TestsPassed);
                }
            }
        }

        //lower is better
        public int AvalancheScore(RngSpecies x)
        {
            return Convert.ToInt32(10*(Math.Abs(x.AvalancheResults.Avalanche - 32) + (x.AvalancheResults.AvalancheRange)));
        }
    }
}