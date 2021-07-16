using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class LinearCongruential16 :Engine16
    {
        private ushort _Multiplier = 0b1010_1010_1010_1011;
        private ushort _Offset = 123;
        private ushort _State = 1;

        public LinearCongruential16()
        {
            _State = (ushort)Engine.Crypto64();
        }

        public LinearCongruential16(UInt64 seed)
        {
            Seed(seed);
        }

        public override ushort Next16()
        {
            _State = (ushort)((ushort)(_State * _Multiplier) + _Offset);
            return _State;
        }

        public override void Seed(ulong seed)
        {
            _State = (ushort)seed;
        }

        public override string ToString()
        {
            return "Linear Congruential Generator";
        }
    }
}
