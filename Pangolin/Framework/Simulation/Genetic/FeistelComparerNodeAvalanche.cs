using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class FeistelComparerNodeAvalanche : IComparer<RngSpecies32Feistel>
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
        public int AvalancheScore(RngSpecies32Feistel x)
        {
            return Convert.ToInt32(Math.Abs(x.AvalancheResults.Avalanche - 32) + (x.AvalancheResults.AvalancheRange));
        }
    }
}
