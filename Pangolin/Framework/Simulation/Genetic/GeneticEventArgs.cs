using System;
using System.Collections.Generic;
using System.Drawing;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class GeneticEventArgs :EventArgs
    {
        public int Generation { set; get; }

        public List<RngSpecies> ThisGeneration { set; get; }

        public Bitmap Image { set; get; }
        
    }
}
