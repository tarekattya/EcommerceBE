using Ecommerce.Application.Helper.Dtos.Cart;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Contarct
{
    public interface ICartService
    {
        Task<Result<CartRequest>> CreateOrUpdateAsync(CartRequest cartRequest);
        Task<Result<CartRequest>> GetCartAsync(string key);

        Task<bool> DeleteAsync(string key);

        

    }
}
