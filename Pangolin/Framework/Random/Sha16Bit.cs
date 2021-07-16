using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;
using MathNet.Numerics;

namespace EnderPi.Framework.Random
{
    public class Sha16Bit : Engine16
    {
        /// <summary>
        /// The cryptographic hash function used to generate random numbers.
        /// </summary>
        [NonSerialized]
        private SHA256Managed _Sha256Managed;
        /// <summary>
        /// The internal state of the generator.  This gets hashed and incremented to generate random numbers.
        /// </summary>
        private BigInteger _State;
        /// <summary>
        /// The results are stored here, since they are generated 4 at a time.
        /// </summary>
        private Queue<ushort> _results;

        public Sha16Bit()
        {
            _State = new BigInteger(1);
            _results = new Queue<ushort>();
        }

        public override ushort Next16()
        {
            if (_results.Count < 1)
            {
                Generate();
            }
            return _results.Dequeue();            
        }

        private void Generate()
        {
            if (_Sha256Managed == null)
            {
                _Sha256Managed = new SHA256Managed();
            }
            var bytes = _Sha256Managed.ComputeHash(_State.ToByteArray());
            for (int i=0; i < 16;i++)
            {
                _results.Enqueue(BitConverter.ToUInt16(bytes, 2 * i));
            }
            _State++;            
        }

        public override void Seed(ulong seed)
        {
            _State = seed;
        }
    }
}
