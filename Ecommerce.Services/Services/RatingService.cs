namespace Ecommerce.Application;

public class RatingService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IRatingService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<RatingResponse>> AddRatingAsync(string userId, RatingRequest request)
    {
        // Validate rating value
        if (request.Rating < 1 || request.Rating > 5)
            return Result<RatingResponse>.Failure(RatingErrors.InvalidRatingValue);

        // Check if product exists
        var product = await _unitOfWork.Repository<Product>()
            .GetByIdWithSpecAsync(new ProductSpecWithBrandAndCategory(request.ProductId));
        
        if (product is null)
            return Result<RatingResponse>.Failure(RatingErrors.ProductNotFound);

        // Check if user already rated this product
        var existingSpec = new RatingByUserAndProductSpec(userId, request.ProductId);
        var existingRating = await _unitOfWork.Repository<ProductRating>().GetByIdWithSpecAsync(existingSpec);
        
        if (existingRating is not null)
            return Result<RatingResponse>.Failure(RatingErrors.AlreadyRated);

        var user = await _userManager.FindByIdAsync(userId);
        
        var rating = new ProductRating(userId, request.ProductId, request.Rating, request.Review);
        var created = await _unitOfWork.Repository<ProductRating>().AddAsync(rating);
        await _unitOfWork.CompleteAsync();

        var response = new RatingResponse(
            created.Id,
            product.Id,
            product.Name,
            user?.DisplayName ?? "Anonymous",
            created.Rating,
            created.Review,
            created.CreatedAt
        );

        return Result<RatingResponse>.Success(response);
    }

    public async Task<Result<RatingResponse>> UpdateRatingAsync(string userId, int ratingId, RatingRequest request)
    {
        // Validate rating value
        if (request.Rating < 1 || request.Rating > 5)
            return Result<RatingResponse>.Failure(RatingErrors.InvalidRatingValue);

        var spec = new RatingByIdSpec(ratingId);
        var rating = await _unitOfWork.Repository<ProductRating>().GetByIdWithSpecAsync(spec);

        if (rating is null)
            return Result<RatingResponse>.Failure(RatingErrors.RatingNotFound);

        if (rating.UserId != userId)
            return Result<RatingResponse>.Failure(RatingErrors.NotAuthorized);

        var user = await _userManager.FindByIdAsync(userId);

        rating.Rating = request.Rating;
        rating.Review = request.Review;

        _unitOfWork.Repository<ProductRating>().Update(rating);
        await _unitOfWork.CompleteAsync();

        var response = new RatingResponse(
            rating.Id,
            rating.ProductId,
            rating.Product.Name,
            user?.DisplayName ?? "Anonymous",
            rating.Rating,
            rating.Review,
            rating.CreatedAt
        );

        return Result<RatingResponse>.Success(response);
    }

    public async Task<Result> DeleteRatingAsync(string userId, int ratingId)
    {
        var rating = await _unitOfWork.Repository<ProductRating>().GetByIdAsync(ratingId);

        if (rating is null)
            return Result.Failure(RatingErrors.RatingNotFound);

        if (rating.UserId != userId)
            return Result.Failure(RatingErrors.NotAuthorized);

        _unitOfWork.Repository<ProductRating>().Delete(rating);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result<RatingResponse>> GetUserRatingForProductAsync(string userId, int productId)
    {
        var spec = new RatingByUserAndProductSpec(userId, productId);
        var rating = await _unitOfWork.Repository<ProductRating>().GetByIdWithSpecAsync(spec);

        if (rating is null)
            return Result<RatingResponse>.Failure(RatingErrors.RatingNotFound);

        var user = await _userManager.FindByIdAsync(userId);

        var response = new RatingResponse(
            rating.Id,
            rating.ProductId,
            rating.Product.Name,
            user?.DisplayName ?? "Anonymous",
            rating.Rating,
            rating.Review,
            rating.CreatedAt
        );

        return Result<RatingResponse>.Success(response);
    }

    public async Task<Result<IReadOnlyList<RatingResponse>>> GetProductRatingsAsync(int productId)
    {
        // Check if product exists
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
        if (product is null)
            return Result<IReadOnlyList<RatingResponse>>.Failure(RatingErrors.ProductNotFound);

        var spec = new RatingsByProductSpec(productId);
        var ratings = await _unitOfWork.Repository<ProductRating>().GetAllWithSpecAsync(spec);

        var responses = new List<RatingResponse>();
        foreach (var rating in ratings)
        {
            var user = await _userManager.FindByIdAsync(rating.UserId);
            responses.Add(new RatingResponse(
                rating.Id,
                rating.ProductId,
                rating.Product.Name,
                user?.DisplayName ?? "Anonymous",
                rating.Rating,
                rating.Review,
                rating.CreatedAt
            ));
        }

        return Result<IReadOnlyList<RatingResponse>>.Success(responses);
    }

    public async Task<Result<ProductRatingsSummary>> GetProductRatingsSummaryAsync(int productId)
    {
        // Check if product exists
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
        if (product is null)
            return Result<ProductRatingsSummary>.Failure(RatingErrors.ProductNotFound);

        var spec = new RatingsByProductSpec(productId);
        var ratings = await _unitOfWork.Repository<ProductRating>().GetAllWithSpecAsync(spec);

        var ratingsList = ratings.ToList();
        var totalRatings = ratingsList.Count;
        var averageRating = totalRatings > 0 ? ratingsList.Average(r => r.Rating) : 0;

        var summary = new ProductRatingsSummary(
            productId,
            Math.Round(averageRating, 2),
            totalRatings,
            ratingsList.Count(r => r.Rating == 5),
            ratingsList.Count(r => r.Rating == 4),
            ratingsList.Count(r => r.Rating == 3),
            ratingsList.Count(r => r.Rating == 2),
            ratingsList.Count(r => r.Rating == 1)
        );

        return Result<ProductRatingsSummary>.Success(summary);
    }

    public async Task<Result<IReadOnlyList<RatingResponse>>> GetUserRatingsAsync(string userId)
    {
        var spec = new RatingsByUserSpec(userId);
        var ratings = await _unitOfWork.Repository<ProductRating>().GetAllWithSpecAsync(spec);

        var user = await _userManager.FindByIdAsync(userId);

        var responses = ratings.Select(rating => new RatingResponse(
            rating.Id,
            rating.ProductId,
            rating.Product.Name,
            user?.DisplayName ?? "Anonymous",
            rating.Rating,
            rating.Review,
            rating.CreatedAt
        )).ToList();

        return Result<IReadOnlyList<RatingResponse>>.Success(responses);
    }
}
