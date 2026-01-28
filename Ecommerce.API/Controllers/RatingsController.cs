namespace Ecommerce.API;

public class RatingsController(IRatingService ratingService) : ApiBaseController
{
    private readonly IRatingService _ratingService = ratingService;

    /// <summary>
    /// Add a rating for a product (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RatingResponse>> AddRating([FromBody] RatingRequest request)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _ratingService.AddRatingAsync(UserId, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Update an existing rating (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPut("{ratingId}")]
    public async Task<ActionResult<RatingResponse>> UpdateRating([FromRoute] int ratingId, [FromBody] RatingRequest request)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _ratingService.UpdateRatingAsync(UserId, ratingId, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Delete a rating (requires authentication)
    /// </summary>
    [Authorize]
    [HttpDelete("{ratingId}")]
    public async Task<ActionResult> DeleteRating([FromRoute] int ratingId)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _ratingService.DeleteRatingAsync(UserId, ratingId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Get the current user's rating for a specific product (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("my-rating/{productId}")]
    public async Task<ActionResult<RatingResponse>> GetMyRatingForProduct([FromRoute] int productId)
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _ratingService.GetUserRatingForProductAsync(UserId, productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get all ratings for a specific product (public)
    /// </summary>
    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IReadOnlyList<RatingResponse>>> GetProductRatings([FromRoute] int productId)
    {
        var result = await _ratingService.GetProductRatingsAsync(productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get rating summary for a specific product (public)
    /// </summary>
    [HttpGet("product/{productId}/summary")]
    public async Task<ActionResult<ProductRatingsSummary>> GetProductRatingsSummary([FromRoute] int productId)
    {
        var result = await _ratingService.GetProductRatingsSummaryAsync(productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get all ratings by the current user (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("my-ratings")]
    public async Task<ActionResult<IReadOnlyList<RatingResponse>>> GetMyRatings()
    {
        if (string.IsNullOrEmpty(UserId))
            return Unauthorized();

        var result = await _ratingService.GetUserRatingsAsync(UserId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
