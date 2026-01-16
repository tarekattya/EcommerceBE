using System.Text.Json;
using StackExchange.Redis;

namespace Ecommerce.Application;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        if (response == null) return;

        var serializedResponse = JsonSerializer.Serialize(response, _jsonOptions);
        await _db.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _db.StringGetAsync(cacheKey);
        
        if (cachedResponse.IsNullOrEmpty) return null;

        return cachedResponse;
    }

    public async Task RemoveCacheByPrefixAsync(string prefix)
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*");
        
        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
