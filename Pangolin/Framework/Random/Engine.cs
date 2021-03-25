using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Security.Cryptography;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Abstract class for a pseudorandom generator engine.
    /// </summary>
    /// <remarks>
    /// This class provides some basic implementation of common features, such as the conversion
    /// from a UInt64 to a double.  All engines must implement Next64().
    /// </remarks>
    [Serializable()]
    public abstract class Engine : IEngine
    {
        /// <summary>
        /// A static cryptographic random number generator.  Used in many seeding algorithms.
        /// </summary>
        public static RNGCryptoServiceProvider _RngCrypto;

        /// <summary>
        /// This static constructor initializes the cryptographic random number generator.
        /// </summary>
        static Engine()
        {
            _RngCrypto = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Gets a cryptographically secure UInt64.  Useful for seeding.
        /// </summary>
        /// <returns>A UInt64</returns>
        public static UInt64 Crypto64()
        {
            Byte[] bytes = new byte[8];
            _RngCrypto.GetBytes(bytes);      //get some random bytes
            return BitConverter.ToUInt64(bytes, 0);
        }

        /// <summary>
        /// This abstract function defines how the engine is seeded from a UInt64 that is a source
        /// of randomness. 
        /// </summary>
        /// <param name="seed">A UInt64</param>
        public abstract void Seed(UInt64 seed);

        /// <summary>
        /// Seeds an engine from an array of UInt64.  
        /// </summary>
        /// <remarks>
        /// The default implementation XOR's all elements of the array together and calls Seed(x) 
        /// with the result.
        /// </remarks>
        /// <param name="seed">An array of UInt64s to use as the seed</param>
        public virtual void Seed(UInt64[] seed)
        {
            UInt64 x = 0;
            foreach (var item in seed)              //Mix all the items together
            {
                x ^= item;
            }
            Seed(x);
        }

        /// <summary>
        /// Discards the next n pseudorandom numbers from the generator.
        /// </summary>
        /// <remarks>
        /// The default implementation calls Next64() n times.
        /// </remarks>
        /// <param name="n">The number of values to discard</param>
        public virtual void Discard(UInt64 n)
        {
            for (ulong i = 0; i < n; i++)
            {
                Next64();
            }
        }

        /// <summary>
        /// Get the next pseudorandom UInt64.  
        /// </summary>
        /// <remarks>
        /// This is the core of every pseudorandom engine.  All other values derive from this.
        /// </remarks>
        /// <returns>A pseudorandom UInt64</returns>
        public abstract UInt64 Next64();

        /// <summary>
        /// Gets an unsigned 32-bit integer.
        /// </summary>
        /// <remarks>
        /// This method is implemented by calling Convert.ToUInt32(), after masking the lower 
        /// 32 bits of Next64()
        /// </remarks>
        /// <returns>A pseudorandom UInt32</returns>
        public UInt32 Next32()
        {
            return Convert.ToUInt32(Next64() & 0xFFFFFFFF);     //mask to get the lower 32 bits only
        }

        /// <summary>
        /// Returns a bit array with the required number of random bits.  Discards the remainder (%64) that don't fit in n.
        /// </summary>
        /// <param name="n">The number of random bits to get.  Limited to ~2 billion by virtue of being an Int32</param>
        /// <returns>A BitArray collection with the required number of bits.</returns>
        public System.Collections.BitArray GetBits(int n)
        {
            System.Collections.BitArray bitArray = new System.Collections.BitArray(n);
            Int32 quotient = n / 64;
            Int32 remainder = n % 64;
            UInt64 next;                        //temporary storage
            for (int i = 0; i < quotient; i++)      //handle the quotient first
            {
                next = Next64();        //get the next lump of 64 unsigned bits
                for (int j = 0; j < 64; j++)
                {
                    bitArray[i * 64 + j] = ((next >> j) & 1UL) == 1UL;      //This syntax is almost too clever
                }
            }
            next = Next64();            //and handle the remainder
            for (int j = 0; j < remainder; j++)
            {
                bitArray[quotient * 64 + j] = ((next >> j) & 1UL) == 1UL;      //This syntax is almost too clever
            }
            return bitArray;
        }

        /// <summary>
        /// Gets the next pseudorandom double on the interval [0,1).  
        /// </summary>
        /// <returns>A pseudorandom double</returns>
        public double NextDouble()        // [0,1)
        {
            return (Next64() >> 11) * (1.0 / 9007199254740992.0);
        }

        /// <summary>
        /// Gets the next psuedorandom double on the interval [0,1].
        /// </summary>
        /// <returns>A pseudorandom double</returns>
        public double NextDoubleInclusive()        // [0,1]        
        {
            return (Next64() >> 11) * (1.0 / 9007199254740991.0);
        }

        /// <summary>
        /// Gets the next psuedorandom double on the interval [0,1].
        /// </summary>
        /// <returns>A pseudorandom double</returns>
        public double NextDoubleInclusive(double lower, double upper)        // [0,1]        
        {
            return NextDoubleInclusive() * (upper - lower) + lower;
        }

        /// <summary>
        /// Gets the next pseudorandom double on the interval (0,1).
        /// </summary>
        /// <returns>A pseudorandom double</returns>
        public double NextDoubleExclusive()
        {
            return ((Next64() >> 12) + 0.5) * (1.0 / 4503599627370496.0);
        }

        /// <summary>
        /// Returns the next int in the given range, inclusive
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public ulong Next64(UInt64 lower, UInt64 upper)
        {
            if (upper == lower)
            {
                return upper;
            }
            UInt64 range = upper - lower + 1;
            UInt64 divided = UInt64.MaxValue / range;
            UInt64 max = divided * range;
            UInt64 random = Next64();
            while (random > max)
            {
                random = Next64();
            }
            return random % range + lower;
        }

        /// <summary>
        /// Returns the next int in the given range, inclusive
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public uint Next32(uint lower, uint upper)
        {
            if (upper == lower)
            {
                return upper;
            }
            uint range = upper - lower + 1;
            uint divided = Int32.MaxValue / range;
            uint max = divided * range;
            uint random = Next32();
            while (random > max)
            {
                random = Next32();
            }
            return random % range + lower;
        }

        public void Shuffle<T>(IList<T> list)
        {
            //To shuffle an array a of n elements(indices 0..n - 1):
            //for i from n−1 downto 1 do
            //        j ← random integer such that 0 ≤ j ≤ i
            //        exchange a[j] and a[i]
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = (int)Next32(0, (uint)i);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public int NextInt(int lower, int upper)
        {
            return (int)Next32((uint)lower, (uint)upper);
        }

        /// <summary>
        /// Returns a random element from the given list.  Throws if the list is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            var index = NextInt(0, list.Count-1);
            return list[index];
        }

        public T PickRandomElement<T>(params T[] elements)
        {
            return GetRandomElement(elements.ToList());
        }

        public ulong FlipRandomBit(ulong x)
        {
            int leftShift = NextInt(0, 63);
            ulong mask = 1UL << leftShift;
            return x ^ mask;
        }

        /// <summary>
        /// Returns a random bitmap.  Currently tailored to emphasize serial correlation in the underlying generator.  Eats exceptions.
        /// </summary>
        /// <param name="randomsToPlot"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap GetBitMap(int randomsToPlot)
        {
            Bitmap bitmap = new Bitmap(256, 256);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y=0; y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, Color.Blue);
                }
            }
            try
            {
                for (int k = 0; k < randomsToPlot; k++)
                {
                    var bytes = BitConverter.GetBytes(Next64());
                    var bytes2 = BitConverter.GetBytes(Next64());
                    for (int p = 0; p < 8; p++)
                    {
                        bitmap.SetPixel(bytes[p], bytes2[p], Color.Red);
                    }
                }
            }
            catch (Exception)
            { }
            return bitmap;
        }

    }
}
