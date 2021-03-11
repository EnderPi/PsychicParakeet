using System;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// A random number generating engine based on the SHA-256 Cryptographic hash function.  
    /// Absolutely highest quality random numbers available.
    /// </summary>
    /// <remarks>
    /// The internal state is a big integer that gets incremented and hashed to generate 
    /// four random numbers.  Each generator produced is independent and suitable to use 
    /// in a different thread.  ~10^18 such generators are available.  Each has 
    /// ~10^39 random numbers available before the streams start to overlap.
    /// </remarks>
    [Serializable]
    public class Sha256 : Engine
    {
        /// <summary>
        /// Static variable to keep track of how many objects have been created, so that they can all be given 
        /// unique stream ID's.
        /// </summary>
        private static ulong _CurrentObjectCounter;

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
        private ulong[] _Results;
        /// <summary>
        /// Keeps track of where this object is in handing out results [0...3].
        /// </summary>
        private int _Counter;
        /// <summary>
        /// The StreamID for this generator.  Generators with different streamIDs will have independent
        /// streams of random numbers, at least 4*2^128 (~10^39) in length.
        /// </summary>
        private ulong _StreamID;

        /// <summary>
        /// Static constructor.  Initializes the current object counter to zero.
        /// </summary>
        static Sha256()
        {
            _CurrentObjectCounter = 0;
        }

        /// <summary>
        /// Constructor, Always gives an independent generator, by making the StreamID different for each one.
        /// Randomly seeds the generator with a cryptographically secure random number.
        /// </summary>
        public Sha256()
        {
            _Sha256Managed = new SHA256Managed();
            _State = new BigInteger(0);
            _Results = new ulong[4];
            _Counter = 4;
            _StreamID = _CurrentObjectCounter++;
            Seed(Crypto64());                       //random seed
        }

        /// <summary>
        /// Discards the next n random numbers, O(1).
        /// </summary>
        /// <param name="n">The number of values to discard</param>
        public override void Discard(ulong n)
        {
            _State += n / 4;
            for (int i = 0; i < (int)(n % 4); i++)
            {
                Next64();
            }
        }

        /// <summary>
        /// Seeds the generator from a given ulong.  The seed goes in bits 128-191, and the StreamID goes in 
        /// bits 193-255.
        /// </summary>
        /// <param name="seed">An seed value</param>
        public override void Seed(ulong seed)
        {
            _State = 0;         //zero the state
            _State += BigInteger.Pow(2, 193) * _StreamID; //the upper 64 bits are for stream ID
            _State += BigInteger.Pow(2, 128) * seed;        //seed goes in these bits
            _Counter = 4;                   //forces it to discard the current results
        }

        /// <summary>
        /// Seeds the generator from an array.  XOR's all the seeds together, then seeds with the result.
        /// </summary>
        /// <param name="seed">The array to use as a seed</param>
        public override void Seed(ulong[] seed)
        {
            UInt64 x = 0;
            foreach (var item in seed)              //Mix all the items together
            {
                x ^= item;
            }
            Seed(x);
        }

        /// <summary>
        /// Returns the next random UInt64 value.  
        /// </summary>
        /// <returns>A pseudo-random unsigned 64-bit number</returns>
        public override ulong Next64()
        {
            if (_Counter > 3)
                Generate();
            return _Results[_Counter++];
        }

        public void Inititalize()
        {
            if (_Sha256Managed == null)
            {
                _Sha256Managed = new SHA256Managed();
            }
        }

        /// <summary>
        /// Generates the next 4 random numbers.  
        /// </summary>
        /// <remarks>
        /// Hashes the state, converts the 256-bit hash into 4 unsigned 64-bit integers, and increments 
        /// the state.
        /// </remarks>
        private void Generate()
        {
            if (_Sha256Managed == null)
            {
                _Sha256Managed = new SHA256Managed();
            }
            var result = _Sha256Managed.ComputeHash(_State.ToByteArray());      //get the hash
            _State++;                                                           //increment the state
            _Results = RandomHelper.BytesToUInt64(result);                           //now get the results
            _Counter = 0;
        }

        public override string ToString()
        {
            return "SHA 256 Managed Engine";
        }

        /// <summary>
        /// Returns a string describing the state of the generator
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("State of {0}", _State.ToString()));
            stringBuilder.AppendLine("Results:");
            foreach (ulong x in _Results)
            {
                stringBuilder.AppendLine(x.ToString());
            }
            stringBuilder.AppendLine(string.Format("Counter of {0}", _Counter));

            return stringBuilder.ToString();
        }

    }
}
