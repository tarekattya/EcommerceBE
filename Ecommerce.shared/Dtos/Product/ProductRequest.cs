
namespace Ecommerce.Shared.Helper.Dtos.Product
{
    public record ProductRequest(string Name,
       string Description,
       string PictureUrl,
       decimal Price,
        int BrandId,
        int CategoryId
        );
    
    
}
