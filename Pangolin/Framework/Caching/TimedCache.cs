using System;
using System.Collections.Concurrent;
using System.Timers;

namespace EnderPi.Framework.Caching
{
    /// <summary>
    /// Simple cache that invalidates periodically.  All methods are thread-safe.
    /// </summary>
    public class TimedCache : ICache
    {
        private ConcurrentDictionary<string, object> _cache;
        private Timer _cacheTimeout;
        private int _millisecondsTimeout;

        public TimedCache(int secondsToLive = 60 * 15)
        {
            _millisecondsTimeout = secondsToLive * 1000;
            _cache = new ConcurrentDictionary<string, object>();
            _cacheTimeout = new Timer(_millisecondsTimeout);
            _cacheTimeout.Elapsed += DropCache;
            _cacheTimeout.AutoReset = true;
            _cacheTimeout.Enabled = true;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DropCache(object source, ElapsedEventArgs e)
        {
            _cache.Clear();
        }

        /// <summary>
        /// Get the object form the cache if it exists, or use the delegate to retrieve the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="populate"></param>
        /// <returns></returns>
        public T Fetch<T>(string key, Func<T> populate)
        {
            if (_cache.TryGetValue(key, out object val))
            {
                return (T)val;
            }
            else
            {
                return (T)_cache.GetOrAdd(key, (string s) => populate());
            }
        }
    }
}
