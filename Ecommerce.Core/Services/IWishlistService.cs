namespace Ecommerce.Core;

public interface IWishlistService
{
    Task<Result<IReadOnlyList<WishlistItemResponse>>> GetUserWishlistAsync(string userId);
    Task<Result<WishlistItemResponse>> AddToWishlistAsync(string userId, WishlistItemRequest request);
    Task<Result> RemoveFromWishlistAsync(string userId, int productId);
    Task<Result<bool>> IsInWishlistAsync(string userId, int productId);
    Task<Result> ClearWishlistAsync(string userId);
}
