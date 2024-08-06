using LW.Cache.Interfaces;
using LW.Contracts.Common;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace LW.Cache;

public class RedisCache<T> : IRedisCache<T> where T : class
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public RedisCache(IDistributedCache redisCacheService, ISerializeService serializeService, IConnectionMultiplexer connectionMultiplexer)
    {
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        _serializeService = serializeService ?? throw new ArgumentNullException(nameof(serializeService));
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));;
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
    public async Task<IEnumerable<T>> GetAllKeysByPattern(string pattern, string field, string value)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return null;
        }

        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).ToArray();

        var result = new List<T>();
        foreach (var key in keys)
        {
            var valueKey = await _redisCacheService.GetStringAsync(key).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(valueKey))
            {
                var deserializedValue = _serializeService.Deserialize<T>(valueKey);
                if (deserializedValue != null)
                {
                    var fieldProperty = deserializedValue.GetType().GetProperty(field)?.GetValue(deserializedValue)?.ToString();
                    if (fieldProperty != null && fieldProperty.Equals(value.Trim()))
                    {
                        result.Add(deserializedValue);
                    }
                }
            }
        }

        return result;
    }
}