using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using StackExchange.Redis;
using System.Text.Json;

namespace Ecommerce.Infrastructure
{
    public class CartRepository(IConnectionMultiplexer connection) : ICartRepository
    {
        public IDatabase _db => connection.GetDatabase();
        public async Task<bool> DeleteCart(string id)
        {
          return await _db.KeyDeleteAsync(id);
        }

        public async Task<CUstomerCart> GetCart(string key)
        {
            var data = await  _db.StringGetAsync(key);
            if (data.IsNullOrEmpty)
                return null!;
            var cart = System.Text.Json.JsonSerializer.Deserialize<CUstomerCart>(data!);
            if (cart is null)
                return null!;
            return cart;
        }
        
        public async Task<CUstomerCart> UpdateOrCreateCart(CUstomerCart cart, TimeSpan? time = null)
        {
            var cartjson =  JsonSerializer.Serialize(cart);
            var createdOrUpdated = await _db.StringSetAsync(cart.Id, cartjson , time ?? TimeSpan.FromDays(30));
            if (createdOrUpdated)
                return await GetCart(cart.Id);
            else
                return null!;
        }
    }
}
