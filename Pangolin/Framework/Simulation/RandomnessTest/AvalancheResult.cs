using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    [Serializable]
    public class AvalancheResult
    {
        /// <summary>
        /// The average number of output bits flipped when an input bit flips.  32 ideal.
        /// </summary>
        public double Avalanche { set; get; }

        /// <summary>
        /// The range across the 64 bits.  Lower values better.
        /// </summary>
        public double AvalancheRange { set; get; }
    }
}
