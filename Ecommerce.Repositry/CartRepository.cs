using System.Text.Json;
using StackExchange.Redis;
using Ecommerce.Core;

namespace Ecommerce.Infrastructure;

public class CartRepository : ICartRepository
{
    private readonly IDatabase _db;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public CartRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string BuildKey(string cartId) => $"cart:{cartId}";

    public async Task<CustomerCart?> GetCartAsync(string cartId)
    {
        if (string.IsNullOrWhiteSpace(cartId))
            return null;

        var data = await _db.StringGetAsync(BuildKey(cartId));
        if (data.IsNullOrEmpty)
            return null;

        var cart = JsonSerializer.Deserialize<CustomerCart>(data.ToString()!, _jsonOptions);
        return cart;
    }

    public async Task<CustomerCart?> CreateCartAsync(CustomerCart cart, TimeSpan? time = null)
    {
        if (string.IsNullOrWhiteSpace(cart.Id))
            return null;

        var exists = await _db.KeyExistsAsync(BuildKey(cart.Id));
        if (exists)
            return null; 

        var json = JsonSerializer.Serialize(cart, _jsonOptions);
        var created = await _db.StringSetAsync(
            BuildKey(cart.Id),
            json,
            time ?? TimeSpan.FromDays(30)
        );

        return created ? cart : null;
    }

    public async Task<CustomerCart?> UpdateCartAsync(CustomerCart cart, TimeSpan? time = null)
    {
        if (string.IsNullOrWhiteSpace(cart.Id))
            return null;

        var exists = await _db.KeyExistsAsync(BuildKey(cart.Id));
        if (!exists)
            return null; 

        var json = JsonSerializer.Serialize(cart, _jsonOptions);
        var updated = await _db.StringSetAsync(
            BuildKey(cart.Id),
            json,
            time ?? TimeSpan.FromDays(30)
        );

        return updated ? cart : null;
    }

    public async Task<bool> DeleteCartAsync(string cartId)
    {
        if (string.IsNullOrWhiteSpace(cartId))
            return false;

        return await _db.KeyDeleteAsync(BuildKey(cartId));
    }
}
