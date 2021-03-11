using System;
using System.Security.Cryptography;

namespace EnderPi.Framework.Random
{
    public class Cryptographic : Engine
    {
        private RNGCryptoServiceProvider _generator;

        public Cryptographic()
        {
            _generator = new RNGCryptoServiceProvider();

        }

        public override ulong Next64()
        {
            byte[] bytes = new byte[8];
            _generator.GetBytes(bytes);      //get some random bytes
            return BitConverter.ToUInt64(bytes, 0);            
        }

        public override void Seed(ulong seed)
        {
            
        }
    }
}
