using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace UltraBend.Common
{
    /// <summary>
    /// Author: Charles Cook
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ExpiringCache<T>
    {
        public static T Cached(MemoryCache cache, string key, TimeSpan? duration, Func<T> getFreshValue, bool forceSet)
        {
            // need to share locks between async and sync methods, so call the async implementation.
            Func<Task<T>> getFreshValueAsync = () => Task.Factory.StartNew(() => getFreshValue());
            return AsyncContext.Run(async () => await ExpiringCache<T>.CachedAsync(cache, key, duration, getFreshValueAsync, forceSet));
        }

        public static async Task<T> CachedAsync(MemoryCache cache, string key, TimeSpan? duration, Func<Task<T>> getFreshValue, bool forceSet)
        {
            // check that the type is serializable
            if (typeof(T).GetInterfaces().Any(t => t.IsGenericType && t.GetGenericArguments().Any(a => !a.IsSerializable)))
            {
                throw new ArgumentException($"The interfaced data being cached, {typeof(T).FullName}, is not serializable");
            }
            else if (typeof(T).GetInterfaces().Count() == 0 && !typeof(T).IsSerializable)
            {
                throw new ArgumentException($"The data being cached, {typeof(T).FullName}, is not serializable");
            }

            if (cache == null)
            {
                return await getFreshValue();
            }

            // the helper is used to handle caching of nullable types as a null check would cause getFreshValue to always be called
            string keyHelper = "Helper_" + key;

            // prevent the possibility of a key clash if someone were to start with Helper_;
            key = "Key_" + key;

            if (duration == null)
            {
                duration = new TimeSpan(0, 20, 0);
            }

            // without a lock try and get the value, this is useful in that flood of calls can concurrently fetch values at an extremely high rate
            if (cache[keyHelper] != null && !forceSet)
            {
                if (cache[key] != null)
                {
                    return (T)cache[key];
                }

                return default(T);
            }

            // no value, get a *unique lock to the particular key* to safely write this particular value
            using (await TypeLock<string>.Get(key).LockAsync())
            {
                // re-attempt the fetch here in case the thread was sitting at the lock waiting for it's release while the value was being stored.
                if (cache[keyHelper] != null && !forceSet)
                {
                    if (cache[key] != null)
                    {
                        return (T)cache[key];
                    }

                    return default(T);
                }

                // write the new value, where an exception in the function call results in storing a null value
                try
                {
                    // check that the function is not null to prevent unnecessary exception handling.
                    if (getFreshValue != null)
                    {
                        var returnValue = await getFreshValue();

                        cache.Set(key, returnValue, DateTime.Now.Add(duration ?? new TimeSpan(0, 5, 0)));
                        
                        cache.Set(keyHelper, true, DateTime.Now.Add(duration ?? new TimeSpan(0, 5, 0)));

                        return returnValue;
                    }
                    else
                    {
                        cache.Set(keyHelper, true, DateTime.Now.Add(duration ?? new TimeSpan(0, 5, 0)));

                        return default(T);
                    }
                }
                catch (Exception exception)
                {
                    // explicitly throw exception
                    throw exception;
                    /*cache.SetCacheItem(keyHelper, true, Convert.ToInt32(duration.TotalMinutes));

                    return default(T);*/
                }
            }
        }

        private static class TypeLock<TKey>
        {
            private static readonly object SyncLock = new object();

            private static readonly SortedDictionary<TKey, AsyncLock> Locks = new SortedDictionary<TKey, AsyncLock>();

            public static AsyncLock Get(TKey index)
            {
                lock (SyncLock)
                {
                    AsyncLock result;
                    if (Locks.TryGetValue(index, out result))
                    {
                        return result;
                    }

                    result = new AsyncLock();
                    Locks[index] = result;
                    return result;
                }
            }
        }
    }
}
