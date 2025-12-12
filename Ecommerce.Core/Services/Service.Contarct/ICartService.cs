
namespace Ecommerce.Core;

    public interface ICartService
    {
        Task<Result<CartRequest>> CreateOrUpdateAsync(CartRequest cartRequest);
        Task<Result<CartRequest>> GetCartAsync(string key);

        Task<Result> DeleteAsync(string key);

        

    }

