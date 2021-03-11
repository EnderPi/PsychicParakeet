using System;

namespace EnderPi.Framework.Random
{
    [Serializable]
    public class LinearCongruential : Engine
    {
        private UInt64 _Multiplier = 3935559000370003845;
        private UInt64 _Offset = 2691343689449507681;
        private UInt64 _State = 0;

        public LinearCongruential()
        {
            _State = Engine.Crypto64();
        }

        public LinearCongruential(UInt64 seed)
        {
            Seed(seed);
        }

        public override ulong Next64()
        {
            _State = _State * _Multiplier + _Offset;
            return _State;
        }

        public override void Seed(ulong seed)
        {
            _State = seed | 1;
        }

        public override string ToString()
        {
            return "Linear Congruential Generator";
        }
    }
}