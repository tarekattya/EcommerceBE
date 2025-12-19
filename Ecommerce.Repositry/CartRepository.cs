
using Ecommerce.Shared;

namespace Ecommerce.Infrastructure;

public class CartRepository(IConnectionMultiplexer connection) : ICartRepository
{
    public IDatabase _db => connection.GetDatabase();
    public async Task<bool> DeleteCart(string id)
    {
      return await _db.KeyDeleteAsync(id);
    }

    public async Task<CUstomerCart> GetCart(string key)
    {
        RedisValue data = await _db.StringGetAsync(key);

        if (data.IsNullOrEmpty)
            return null!;

        var cart = JsonSerializer.Deserialize<CUstomerCart>(data.ToString());
        if (cart is null)
            return null!;
        return cart;
    }

    public async Task<CUstomerCart> UpdateOrCreateCart(CUstomerCart cart, TimeSpan? time = null)
    {
        var cartjson =  JsonSerializer.Serialize(cart);
        var createdOrUpdated = await _db.StringSetAsync(cart.Id, cartjson , time ?? TimeSpan.FromDays(30));
        if (createdOrUpdated && cart.Id is not null)
            return await GetCart(cart.Id);
        else
            return null!;
    }
}
