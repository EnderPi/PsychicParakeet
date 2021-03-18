using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// A record in the ChiSquaredPoco table.
    /// </summary>
    public class GcdChiSquaredPoco
    {
        public int SimulationId { set; get; }
        public int Gcd { set; get; }
        public long Count { set; get; }
        /// <summary>
        /// The expected count.  
        /// </summary>
        public double ExpectedCount { set; get; }
        public double FractionOfChiSquared { set; get; }        
    }
}
