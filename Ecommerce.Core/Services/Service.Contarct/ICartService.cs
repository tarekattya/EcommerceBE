


using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Helper.Dtos.Cart;

namespace Ecommerce.Core.Services.Service.Contarct
{
    public interface ICartService
    {
        Task<Result<CartRequest>> CreateOrUpdateAsync(CartRequest cartRequest);
        Task<Result<CartRequest>> GetCartAsync(string key);

        Task<Result> DeleteAsync(string key);

        

    }
}
