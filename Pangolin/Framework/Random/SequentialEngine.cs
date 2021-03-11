using System;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Obviously this is not really a random engine.  It mostly exists for testing purposes.
    /// </summary>
    public class SequentialEngine :Engine
    {
        private ulong _state;

        public override ulong Next64()
        {
            return _state++;
        }

        public override void Seed(ulong seed)
        {
            _state = seed;
        }

        public override string ToString()
        {
            return "Sequential Generator";
        }
    }
}
