using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class Romul :Engine
    {
        private ulong _state;
        private ulong _multiplier;
        private ulong _last;
        private int _rotate;
        private ulong _initialMultiplier;
        private bool _skip;

        public Romul() : this(1518061951166098093, 29) { }

        public Romul(ulong multiplier, int rotate)
        {
            _multiplier = multiplier;
            _initialMultiplier = multiplier;
            _rotate = rotate;
            Seed(Engine.Crypto64());
        }


        public override ulong Next64()
        {
            //rotate of 21 fails maurer bytewise, but is otherwise good.
            //rotate of 31 fails maurer bytewise at 7.34 billion
            //rotate of 30.....passes maurer bytewise at 10 billion, passes linear complexity at 20,000
                        
            _state = BitOperations.RotateLeft(_state * _multiplier, _rotate);
            if (_state == _last)
            {
                _multiplier += 2;                
                _skip = !_skip;
                if (!_skip)
                {
                    return Next64();
                }
            }
            return _state;

        }               

        public override void Seed(ulong seed)
        {
            _state = seed;
            if (_state == 0)
            {
                _state = 1;
            }
            _multiplier = _initialMultiplier;
            _last = _state;
            _skip = false;
        }
    }
}
