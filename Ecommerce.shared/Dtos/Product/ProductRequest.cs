namespace Ecommerce.Shared;
public record ProductRequest(string Name, string Description, decimal Price, int BrandId, int CategoryId, int Stock, string? PictureUrl = null);
