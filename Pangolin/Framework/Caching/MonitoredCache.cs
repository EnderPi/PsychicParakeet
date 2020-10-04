using System;
using System.Collections.Concurrent;
using EnderPi.Framework.Interfaces;
using EnderPi.Framework.Messaging.Events;


namespace EnderPi.Framework.Caching
{
    /// <summary>
    /// This cache is monitored!  It uses the pub-sub framework to listen for cache invalidation events, and drops the cached data.  Also has an event hook.
    /// </summary>
    /// <remarks>
    /// It's not super clear to me that this class is necessary.  You could create strongly type cached providers that handled details better, with strongly typed cache invalidation events, right?
    /// Also, this class can leak an application event subscription, so....food for thought.
    /// </remarks>
    public class MonitoredCache : ICache
    {
        /// <summary>
        /// Private dictionary that holds the cache.
        /// </summary>
        private ConcurrentDictionary<string, object> _cache;

        /// <summary>
        /// The name of this cache.
        /// </summary>
        private string _cacheName;

        /// <summary>
        /// The delegate architecture for event handlers.
        /// </summary>
        /// <param name="sender">The Monitored Cache reference.</param>
        /// <param name="e">Any relevant parameters.</param>
        public delegate void MonitoredCacheEventHandler(object sender, CacheInvalidationEventArgs e);

        /// <summary>
        /// The event that occurs when the cache drops.
        /// </summary>
        public event MonitoredCacheEventHandler CacheDropped;

        /// <summary>
        /// Occurs when the cache is dropped.
        /// </summary>
        private void OnCacheInvalidated()
        {
            CacheDropped?.Invoke(this, new CacheInvalidationEventArgs());
        }

        /// <summary>
        /// Basic constructor.  Initializes state and subscribes to cache invalidation events.
        /// </summary>
        /// <param name="cacheName">Cache name, this will be checked against cache invalidation events when they occur.</param>
        /// <param name="eventManager">The IEventManager that will be used to subscribe to cache invalidation events.</param>
        public MonitoredCache(string cacheName, IEventManager eventManager)
        {
            _cache = new ConcurrentDictionary<string, object>();
            _cacheName = cacheName;
            eventManager.Subscribe<CacheInvalidationEvent>(DropCache);
        }

        /// <summary>
        /// Delegate to process cache invalidation events.
        /// </summary>
        /// <param name="e"></param>
        private void DropCache(CacheInvalidationEvent e)
        {
            if (string.Equals(e.CacheName, _cacheName, StringComparison.OrdinalIgnoreCase))
            {
                //todo deal with how I might want to use cache key invalidation.  Kind of weird to handle since the cache user is who dictates the cache key.
                //I suppose that would only work great if you had a super strongly typed monitored cache?
                //like a cachedglobalsettingsprovider?  
                //TODO this thing leaks an application subscription, which could be problematic for tests, but probably not for production.
                //Something to think aboot.
                //A leaked application subscription isn't really a problem, since the event manager will basically discard the event
                //hmmmmmmmmm......
                //it's only really a problem in a unit test, where a queue will be flooded, and never emptied, since it's a test queue.
                //might be a good argument for a separate unit test database.
                _cache.Clear();
                OnCacheInvalidated();
            }
        }

        /// <summary>
        /// ICache implementation to fetch an object from the cache or the delegate.
        /// </summary>
        /// <typeparam name="T">The type of object to fetch.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="populate">The delegate to populate the cache if needed.</param>
        /// <returns></returns>
        public T Fetch<T>(string key, Func<T> populate)
        {
            object val;
            if (_cache.TryGetValue(key, out val))
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
