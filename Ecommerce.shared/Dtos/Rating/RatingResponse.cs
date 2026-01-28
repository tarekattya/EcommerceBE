namespace Ecommerce.Shared;

public record RatingResponse(
    int Id,
    int ProductId,
    string ProductName,
    string UserName,
    int Rating,
    string? Review,
    DateTime CreatedAt
);
