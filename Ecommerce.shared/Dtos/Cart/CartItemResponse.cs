namespace Ecommerce.Shared;

public record CartItemResponse(
    int VariantId,
    int ProductId,
    string? Size,
    string? Color,
    int Quantity,
    decimal Price,
    string ProductName,
    string Brand,
    string Type,
    string PictureUrl
);

