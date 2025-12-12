namespace Ecommerce.Shared;
public record ProductRequest(string Name,string Description,string PictureUrl, decimal Price,int BrandId, int CategoryId);
