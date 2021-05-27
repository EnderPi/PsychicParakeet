using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    public class MultiplyRotateResult
    {
        public uint Multiplier { set; get; }

        public int Rotate { set; get; }

        public uint Period { set; get; }

        public int[] Linearity { set; get; }
    }
}
