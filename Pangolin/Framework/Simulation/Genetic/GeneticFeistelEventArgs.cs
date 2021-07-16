using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class GeneticFeistelEventArgs
    {
        public int Generation { set; get; }

        public List<RngSpecies32Feistel> ThisGeneration { set; get; }

        public Bitmap Image { set; get; }

        public int Iteration { set; get; }
    }
}
