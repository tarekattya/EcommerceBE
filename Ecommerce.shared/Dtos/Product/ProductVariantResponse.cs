namespace Ecommerce.Shared;

public record ProductVariantResponse(int Id, int ProductId, string? Size, string? Color, string SKU, decimal Price, int Stock);
