using Ecommerce.API.Abstractions;
using Ecommerce.Shared.Helper.Dtos.Cart;
using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.shared.Abstraction.Errors.Cart;
using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Services.Service.Contarct;

namespace Ecommerce.API.Controllers
{
    public class CartsController(ICartService cartService) : ApiBaseController
    {
        private readonly ICartService _cartService = cartService;

        [HttpGet("")]
        public async Task<ActionResult<CartRequest>> GetCart(string id)
        {
            var result = await _cartService.GetCartAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }


        [HttpPost]
        public async Task<ActionResult<CartRequest>> CreateOrUpdate(CartRequest cartRequest)
        {
            var result = await _cartService.CreateOrUpdateAsync(cartRequest);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

        }


        [HttpDelete]
        public async Task<ActionResult<CartRequest>> DeleteCart(string id)
        {
            var result = await _cartService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }


    }
}
