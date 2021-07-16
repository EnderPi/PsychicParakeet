using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class Romul16 : Engine16
    {
        private ushort _state;
        private ushort _multiplier;
        private ushort _last;
        private int _rotate;
        private ushort _initialMultiplier;
        private bool _skip;

        public Romul16() : this(43691,11) { }

        public Romul16(ushort multiplier, int rotate)
        {
            _multiplier = multiplier;
            _initialMultiplier = multiplier;
            _rotate = rotate;
            Seed(Engine.Crypto64());
        }

        public override ushort Next16()
        {
            _state = RandomHelper.RotateLeft((ushort)(_state * _multiplier), _rotate);
            if (_state == _last)
            {
                _multiplier += 2;
                _skip = !_skip;
                if (!_skip)
                {
                    return Next16();
                }
            }
            return _state;
        }

        

        public override void Seed(ulong seed)
        {
            _state = Convert.ToUInt16(seed & ushort.MaxValue);
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
