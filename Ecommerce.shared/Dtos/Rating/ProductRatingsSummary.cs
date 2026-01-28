namespace Ecommerce.Shared;

public record ProductRatingsSummary(
    int ProductId,
    double AverageRating,
    int TotalRatings,
    int FiveStarCount,
    int FourStarCount,
    int ThreeStarCount,
    int TwoStarCount,
    int OneStarCount
);
