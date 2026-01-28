namespace Ecommerce.API;

[Authorize]
public class WishlistController(IWishlistService wishlistService) : ApiBaseController
{
    private readonly IWishlistService _wishlistService = wishlistService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WishlistItemResponse>>> GetWishlist()
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _wishlistService.GetUserWishlistAsync(UserId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<ActionResult<WishlistItemResponse>> AddToWishlist([FromBody] WishlistItemRequest request)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _wishlistService.AddToWishlistAsync(UserId, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult> RemoveFromWishlist([FromRoute] int productId)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _wishlistService.RemoveFromWishlistAsync(UserId, productId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("check/{productId}")]
    public async Task<ActionResult<bool>> IsInWishlist([FromRoute] int productId)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _wishlistService.IsInWishlistAsync(UserId, productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearWishlist()
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _wishlistService.ClearWishlistAsync(UserId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
