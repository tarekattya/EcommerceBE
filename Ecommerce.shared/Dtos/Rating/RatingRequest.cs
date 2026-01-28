namespace Ecommerce.Shared;

public record RatingRequest(int ProductId, int Rating, string? Review = null);
