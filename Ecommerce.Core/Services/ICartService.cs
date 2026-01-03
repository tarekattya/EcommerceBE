
namespace Ecommerce.Core;

public interface ICartService
{
    Task<Result<CartResponse>> CreateCartAsync(CartRequest cartRequest);
    Task<Result<CartResponse>> UpdateCartAsync(CartRequest cartRequest);
    Task<Result<CartResponse>> GetCartAsync(string key);
    Task<Result<CartResponse>> RemoveItemsAsync(RemoveCartItemsRequest request);
    Task<Result<CartResponse>> AddItemsAsync(AddCartItemsRequest request);
    Task<Result> DeleteAsync(string key);



}

