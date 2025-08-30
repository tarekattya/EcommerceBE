using Ecommerce.Core.Entites;

namespace Ecommerce.API.Mapping.ProductMap
{
    public record productResponse(
        int id,
        string Name,
       string Description,
       string PictureUrl,
        int BrandId,
        string Brand,
        int CategoryId,
        string Category
        );
    ///

}
