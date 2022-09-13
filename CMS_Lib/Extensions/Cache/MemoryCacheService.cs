using System;
using CMS_Lib.DI;
using Microsoft.Extensions.Caching.Memory;

namespace CMS_Lib.Extensions.Cache
{
    public interface IIMemoryCacheService : IScoped
    {
        T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory) where T : class;
        T GetFromCache<T>(string key) where T : class;
        void SetCache<T>(string key, T value) where T : class;
        void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class;
        void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class;
        void ClearCache(string key);
    }

    public class MemoryCacheService : IIMemoryCacheService
    {
        private const int CacheSeconds = 10;
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public T GetOrCreate<T>(string key,Func<ICacheEntry, T> factory) where T : class
        {
            return _cache.GetOrCreate(key, factory);
        }

        public T GetFromCache<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value) where T : class
        {
            SetCache(key, value, DateTimeOffset.Now.AddSeconds(CacheSeconds));
        }

        public void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class
        {
            _cache.Set(key, value, duration);
        }

        public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class
        {
            _cache.Set(key, value, options);
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
