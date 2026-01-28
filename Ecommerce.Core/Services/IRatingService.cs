namespace Ecommerce.Core;

public interface IRatingService
{
    Task<Result<RatingResponse>> AddRatingAsync(string userId, RatingRequest request);
    Task<Result<RatingResponse>> UpdateRatingAsync(string userId, int ratingId, RatingRequest request);
    Task<Result> DeleteRatingAsync(string userId, int ratingId);
    Task<Result<RatingResponse>> GetUserRatingForProductAsync(string userId, int productId);
    Task<Result<IReadOnlyList<RatingResponse>>> GetProductRatingsAsync(int productId);
    Task<Result<ProductRatingsSummary>> GetProductRatingsSummaryAsync(int productId);
    Task<Result<IReadOnlyList<RatingResponse>>> GetUserRatingsAsync(string userId);
}
