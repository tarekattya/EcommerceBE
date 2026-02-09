namespace Ecommerce.Shared;

public record ProductVariantRequest(string? Size, string? Color, string SKU, decimal Price, int Stock);
