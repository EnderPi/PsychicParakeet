namespace EnderPi.Framework.Caching
{
    /// <summary>
    /// Class that holds cache names for hard-coded caches.
    /// </summary>
    public static class CacheNames
    {
        /// <summary>
        /// This cache holds event subscriptions.  Invalidation of this cache means the publisher needs to refresh subscriptions from the daterbase.
        /// </summary>
        public static string EventSubscriptions = "EventSubscriptions";

        /// <summary>
        /// The Global configurations cache.
        /// </summary>
        public static string GlobalConfigurations = "GlobalConfigurations";
    }
}
