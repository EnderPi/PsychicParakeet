using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    public class LinearGeneticEventArgs
    {
        public int Generation { set; get; }

        public List<LinearGeneticSpecimen> ThisGeneration { set; get; }

        public Bitmap Image { set; get; }

        public int Iteration { set; get; }
    }
}
