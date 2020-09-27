using System;

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
        /// <typeparam name="T">The type of the object being retrieved.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="populate">The delegate to populate the cache if needed.</param>
        /// <returns>The object from the cache, or the delegate if the key isn't in the cache.</returns>
        T Fetch<T>(string key, Func<T> populate);
    }
}
