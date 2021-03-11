using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Interface for a random number generator engine.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Seeds the random number generator.
        /// </summary>
        /// <param name="seed">A UInt64 seed of randomness</param>
        void Seed(UInt64 seed);
        /// <summary>
        /// Seeds the random number generator from an array.  
        /// </summary>
        /// <remarks>
        /// This is primarily used for engines with a large internal state.
        /// </remarks>
        /// <param name="seed">A seed array that is a a source of randomness</param>
        void Seed(UInt64[] seed);
        /// <summary>
        /// Discards numbers from the generator, effectively advancing it N steps.
        /// </summary>
        /// <param name="n">The number of values to discard</param>
        void Discard(UInt64 n);
        /// <summary>
        /// Gets the next random 64-bit number.
        /// </summary>
        /// <returns>A random 64-bit number</returns>
        UInt64 Next64();
        /// <summary>
        /// Gets the next random 32-bit number
        /// </summary>
        /// <returns>A random 32-bit number</returns>
        UInt32 Next32();
        /// <summary>
        /// Gets random bits from the engine.
        /// </summary>
        /// <param name="n">The required number of random bits</param>
        /// <returns>An array of random bits</returns>
        System.Collections.BitArray GetBits(int n);
        /// <summary>
        /// Gets the next random double on [0,1)
        /// </summary>
        /// <returns>A random double on [0,1)</returns>
        double NextDouble();
        /// <summary>
        /// Gets the next random double on [0,1].
        /// </summary>
        /// <returns>A random double on [0,1]</returns>
        double NextDoubleInclusive();
        /// <summary>
        /// Gets the next random number on (0,1).
        /// </summary>
        /// <returns>A random number on (0,1)</returns>
        double NextDoubleExclusive();       //(0,1)
    }
}
