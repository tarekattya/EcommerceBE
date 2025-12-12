namespace Ecommerce.Application;

public class CartService(ICartRepository cartRepository) : ICartService
{
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<Result<CartRequest>> CreateOrUpdateAsync(CartRequest cartRequest)
    {
        var Cart = cartRequest.Adapt<CUstomerCart>();   
        var basket = await _cartRepository.UpdateOrCreateCart(Cart);

        if (basket is not null)
          return await GetCartAsync(cartRequest.Id);
        return Result<CartRequest>.Failure(CartErrors.CantCreateOrUpdate);
    }

    public async Task<Result> DeleteAsync(string key)
    {
       var result = await _cartRepository.DeleteCart(key);
          return result ? Result.Success() : Result.Failure(CartErrors.NotFoundCart);
    }

    public async Task<Result<CartRequest>> GetCartAsync(string key)
    {
        var basket = await _cartRepository.GetCart(key);
        if(basket is not null)
            return Result<CartRequest>.Success(basket.Adapt<CartRequest>());
        return Result<CartRequest>.Failure(CartErrors.NotFoundCart);    
    }
}
