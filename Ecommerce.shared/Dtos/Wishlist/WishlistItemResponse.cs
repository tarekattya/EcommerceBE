namespace Ecommerce.Shared;

public record WishlistItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    string PictureUrl,
    decimal Price,
    string Brand,
    string Category,
    DateTime AddedAt
);
