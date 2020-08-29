using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Caching
{
    /// <summary>
    /// Simple caching interface.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// returns the object from the cache, or populates the cache with the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="populate"></param>
        /// <returns></returns>
        T Fetch<T>(string key, Func<T> populate);
    }
}
