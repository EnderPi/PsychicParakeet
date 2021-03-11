using System;
using System.Collections.Concurrent;

namespace EnderPi.Framework.Services
{
    public class ServiceProvider
    {
        public ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();

        public void RegisterService<T>(T service)
        {
            _services.AddOrUpdate(typeof(T), service, (Type t, object o) => service);
        }

        public T GetService<T>()
        {
            object service;
            if (_services.TryGetValue(typeof(T), out service))
            {
                return (T)service;
            }
            else
            {
                throw new Exception("Service not found.");
            }
        }
    }
}
