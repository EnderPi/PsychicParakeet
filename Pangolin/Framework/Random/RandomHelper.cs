using System;

namespace EnderPi.Framework.Random
{
    public static class RandomHelper
    {
        public static ulong RotateRight(ulong x, int k)
        {
            return (x >> k) | (x << (64 - k));
        }

        public static ulong RotateLeft(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }

        /// <summary>
        /// Converts the given byte array into an array of UInt64s, one-eighth the length.
        /// Used to convert hashes into UInt64s.
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns>An array of UInt64</returns>
        public static ulong[] BytesToUInt64(byte[] b)
        {
            ulong[] result = new ulong[b.Length / 8];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = BitConverter.ToUInt64(b, i * 8);
            }
            return result;
        }

    }
}
