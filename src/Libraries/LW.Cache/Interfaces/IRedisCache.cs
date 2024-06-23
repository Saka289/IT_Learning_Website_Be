using Microsoft.Extensions.Caching.Distributed;

namespace LW.Cache.Interfaces;

public interface IRedisCache<T> where T : class
{
    Task<T?> GetStringKey(string key);

    Task<T> SetStringKey(string key, T value, DistributedCacheEntryOptions options = null);

    Task<bool> RemoveStringKey(string key);
}