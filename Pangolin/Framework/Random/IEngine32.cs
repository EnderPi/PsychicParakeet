using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Interface for 32-bit Random number engines.
    /// </summary>
    public interface IEngine32
    {
        /// <summary>
        /// The next 32-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public uint Next32();
    }
}
