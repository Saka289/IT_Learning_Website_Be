using LW.Cache.Interfaces;
using LW.Contracts.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace LW.Cache;

public class RedisCache<T> : IRedisCache<T> where T : class
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;

    public RedisCache(IDistributedCache redisCacheService, ISerializeService serializeService)
    {
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        _serializeService = serializeService ?? throw new ArgumentNullException(nameof(serializeService));
    }

    public async Task<T?> GetStringKey(string key)
    {
        var result = await _redisCacheService.GetStringAsync(key);
        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        return _serializeService.Deserialize<T>(result);
    }

    public async Task<T> SetStringKey(string key, T value, DistributedCacheEntryOptions options = null)
    {
        if (options != null)
        {
            await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(value), options);
        }
        else
        {
            await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(value));
        }

        return await GetStringKey(key);
    }

    public async Task<bool> RemoveStringKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        await _redisCacheService.RemoveAsync(key);
        return true;
    }
}