using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class GeneticAvalancheFunctionFeistel : IGeneticAvalancheFunction
    {
        private IEngine _engine;
        public GeneticAvalancheFunctionFeistel(IEngine engine)
        {
            _engine = engine;
        }

        public ulong Hash(ulong x)
        {
            _engine.Seed(x);
            return _engine.Next64();            
        }
    }
}
