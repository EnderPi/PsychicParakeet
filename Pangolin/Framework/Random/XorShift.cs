using System;

namespace EnderPi.Framework.Random
{
    [Serializable]
    public class XorShift : Engine
    {
        private ulong _state;
        public override ulong Next64()
        {
            ulong x = _state;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            return _state = x;
        }

        public override void Seed(ulong seed)
        {
            _state = seed | 1UL;
        }

        public override string ToString()
        {
            return "64-bit XOR Shift RNG";
        }
    }
}
