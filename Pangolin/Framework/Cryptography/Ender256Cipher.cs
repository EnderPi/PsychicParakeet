using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EnderPi.Framework.Cryptography
{
    /// <summary>
    /// A cipher algorithm with a 256-bit key, and 256-bit block.
    /// </summary>
    public class Ender256Cipher
    {
        private int[] _LeftKey;
        private int[] _RightKey;
        private byte[] _iv;

        private ulong _multiplier = 1518061951166098093UL;      //chosen for high average avalanche of ~20.2


        public Ender256Cipher(byte[] key, byte[] iv)
        {
            _iv = iv;
            ulong a = BitConverter.ToUInt64(key, 0);
            ulong b = BitConverter.ToUInt64(key, 8);
            ulong c = BitConverter.ToUInt64(key, 16);
            ulong d = BitConverter.ToUInt64(key, 24);
            _LeftKey = GetKeys(a, b);
            _RightKey = GetKeys(c, d);

            //so the iv and key should both be 256 bits.

            //some key expansion.  Needs to be deterministic, obvi
            //use left 128 bits to get left keys, right 128 bits to get right keys.


        }

        private int[] GetKeys(ulong a, ulong b)
        {
            var hasher = new Feistel128Hash();
            return hasher.GetKeys(a, b);            
        }

        /// <summary>
        /// Encrypt the plaintext with the given counter.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="counter">The unsigned long to start with, for 256-bit counter mode.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainText, ulong counter=0)
        {
            byte[] cypherText = new byte[plainText.Length];
            using (MemoryStream ms = new MemoryStream(plainText))
            {
                using (MemoryStream cyphertext = new MemoryStream(cypherText))
                {
                    int bytesRead;
                    var readbytes = new byte[32];
                    do
                    {
                        bytesRead = ms.Read(readbytes);
                        byte[] encrypted = EncryptBlock(bytesRead, readbytes, counter);
                        cyphertext.Write(encrypted);
                        counter++;
                    } while (bytesRead == 32);
                }
            }

                throw new NotImplementedException();

        }

        private byte[] EncryptBlock(int bytesRead, byte[] readbytes, ulong counter)
        {
            var encryptedBytes = new byte[bytesRead];
            ulong[] bytes = new ulong[4];
            ulong temp;
            //encrypt counter and IV

            //xor with bytes

            //return
            return encryptedBytes;
        }

        public byte[] Decrypt(byte[] plainText, uint counter = 0)
        {
            //literally the same as encrypt, but with keys reversed.
            throw new NotImplementedException();
        }


    }
}
