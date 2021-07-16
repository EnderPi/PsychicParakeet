using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class Romul32 :Engine
    {
        private uint _state;
        private uint _last;
        private int _rotate = 17;
        private uint _multiplier = 699192619;
        private bool _skip;

        public override ulong Next64()
        {
            ulong result = Convert.ToUInt64(Next32q()) << 32;
            result = result | Convert.ToUInt64(Next32q());
            return result;
        }

        public Romul32(uint multiplier, int rotate)
        {
            _multiplier = multiplier;
            _rotate = rotate;
        }

        private uint Next32q()
        {
            _state = BitOperations.RotateLeft(_multiplier * _state, _rotate);
            if (_state == _last)
            {
                _multiplier += 2;
                _skip = !_skip;
                if (!_skip)
                {
                    return Next32q();
                }                
            }
            return _state;
        }
                

        public override void Seed(ulong seed)
        {
            _state = Convert.ToUInt32(seed & UInt32.MaxValue);
            if (_state == 0)
            {
                _state = 1;
            }
            _last = _state;
            _skip = false;
        }
    }
}
