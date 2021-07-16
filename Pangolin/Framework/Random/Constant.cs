using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class Constant : Engine
    {
        private ulong _value;
        public override ulong Next64()
        {
            return _value;
        }

        public override void Seed(ulong seed)
        {
            _value = seed;
        }
    }
}
