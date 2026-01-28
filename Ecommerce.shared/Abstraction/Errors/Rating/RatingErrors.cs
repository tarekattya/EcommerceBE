namespace Ecommerce.Shared;

public static class RatingErrors
{
    public static Error RatingNotFound => new Error("RatingNotFound", "Rating not found", statusCode: 404);
    public static Error AlreadyRated => new Error("AlreadyRated", "You have already rated this product. You can update your existing rating.", statusCode: 409);
    public static Error ProductNotFound => new Error("ProductNotFound", "The specified product does not exist", statusCode: 404);
    public static Error InvalidRatingValue => new Error("InvalidRatingValue", "Rating must be between 1 and 5", statusCode: 400);
    public static Error NotAuthorized => new Error("NotAuthorized", "You are not authorized to modify this rating", statusCode: 403);
    public static Error MustPurchaseProduct => new Error("MustPurchaseProduct", "You must purchase this product before rating it", statusCode: 403);
}
