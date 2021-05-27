using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// A simple byte that has an API to make it an array of bits.
    /// </summary>
    public struct NullBitArray8
    {
        private byte _state;

        public NullBitArray8(byte state)
        {
            _state = state;
        }

        public bool this[int index]
        {
            get
            {
                return (_state & (byte)(1 << index)) != 0;
            }
            set
            {
                if (value)
                {
                    _state |= (byte)(1 << index);
                }
                else
                {
                    _state &= (byte)~(byte)(1 << index);
                }
            }
        }

    }
}
