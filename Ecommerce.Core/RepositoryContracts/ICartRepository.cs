
namespace Ecommerce.Core;

    public interface ICartRepository
    {
        public Task<CUstomerCart> GetCart(string key);

        public Task<CUstomerCart> UpdateOrCreateCart(CUstomerCart cart , TimeSpan? time = null);

        public Task<bool> DeleteCart(string id);


    }

