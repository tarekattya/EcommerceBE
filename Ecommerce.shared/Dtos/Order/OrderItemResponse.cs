namespace Ecommerce.Shared;
public record OrderItemResponse(int ProductId, int? ProductVariantId, string? Size, string? Color, string ProductName, string PictureUrl, int Quantity, decimal Price);


