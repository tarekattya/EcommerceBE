namespace Ecommerce.Core;

public interface ICacheService
{
    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
    Task<string?> GetCachedResponseAsync(string cacheKey);
    Task RemoveCacheByPrefixAsync(string prefix);
}
