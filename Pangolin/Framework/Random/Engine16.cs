using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    public abstract class Engine16
    {
        public abstract ushort Next16();

        public abstract void Seed(ulong seed);

    }
}
