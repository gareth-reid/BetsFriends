using System;
using System.Runtime.Caching;

namespace BF_API.CacheManager
{
    public class CacheManager<T> : ICacheManager<T>
    {
        static readonly ObjectCache tokenCache = MemoryCache.Default;
        public CacheItem TokenContents;        

        public T GetItem(string key)
        {
            CacheItem tokenContents = tokenCache.GetCacheItem(key);
            if (tokenContents != null)
            {
                return (T)tokenContents.Value;
            }
            return default(T);                          
            
        }

        public bool SetItem(string key, T data, DateTimeOffset timeToDirty)
        {            
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = timeToDirty;
            CacheItem tokenContents = new CacheItem(key, data);
            tokenCache.Set(tokenContents, policy);
            return true;
        }
    }
}
;