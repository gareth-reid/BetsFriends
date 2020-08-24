using System;
namespace BF_API.CacheManager
{
    public interface ICacheManager<T>
    {
        T GetItem(string key);
        bool SetItem(string key, T data, DateTimeOffset timeToDirty);
    }
}
