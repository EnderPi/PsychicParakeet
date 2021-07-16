using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    /// <summary>
    /// Calculates the avalanche of a given function ulong->ulong
    /// </summary>
    public static class AvalancheCalculator
    {
        public static AvalancheResult GetAvalanche(Engine randomEngineForSeeds, IGeneticAvalancheFunction function, int seedsToTest)
        {            
            var totalBitsFlipped = new double[64];
            for (int j = 0; j < seedsToTest; j++)
            {
                ulong thingToHash = randomEngineForSeeds.Next64();
                ulong before = function.Hash(thingToHash);
                for (int k = 0; k < 64; k++)
                {
                    ulong mask = 1UL << k;
                    ulong after = function.Hash(thingToHash ^ mask);
                    ulong xor = before ^ after;
                    var count = TestHelper.CountBits(xor);
                    totalBitsFlipped[k] += count;
                }
            }
            var result = new AvalancheResult();
            result.Avalanche = totalBitsFlipped.Average() / seedsToTest;
            double max = totalBitsFlipped.Max();
            double min = totalBitsFlipped.Min();
            result.AvalancheRange = (max - min) / seedsToTest;

            return result;
        }
    }
}
