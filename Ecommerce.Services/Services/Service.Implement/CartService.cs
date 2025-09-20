using Ecommerce.Application.Helper.Dtos.Cart;
using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Core.Entites;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.shared.Abstraction.Errors.Cart;
using Ecommerce.Shared.Abstraction;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Implement
{
    public class CartService(ICartRepository cartRepository) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;

        public async Task<Result<CartRequest>> CreateOrUpdateAsync(CartRequest cartRequest)
        {
            var Cart = cartRequest.Adapt<CUstomerCart>();
            var basket = await _cartRepository.UpdateOrCreateCart(Cart);

            if (basket is not null)
              return await GetCartAsync(cartRequest.Id);
            return Result<CartRequest>.Failure(CartErrors.NotFoundCart);



        }

        public async Task<bool> DeleteAsync(string key)
        {
           return  await _cartRepository.DeleteCart(key);
        }

        public async Task<Result<CartRequest>> GetCartAsync(string key)
        {
            var basket = await _cartRepository.GetCart(key);
            if(basket is not null)
                return Result<CartRequest>.Success(basket.Adapt<CartRequest>());
            return Result<CartRequest>.Failure(CartErrors.CantCreateOrUpdate);    
        }
    }
}
