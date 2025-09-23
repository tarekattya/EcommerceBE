using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Shared.Helper.Dtos.Product;
using Mapster;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.Arm;

namespace Ecommerce.API.Mapping.ProductMap
{
    public class Config
    {
        public static void Register(TypeAdapterConfig config, IOptions<BaseUrl> urlOptions)
        {
            var baseUrl = urlOptions.Value.BaseURL;
            config.NewConfig<Product, productResponse>()
                   .Map(dest => dest.Brand, src => src.Brand.Name)
                   .Map(dest => dest.Category, src => src.Category.Name)
                  .Map(dest => dest.PictureUrl,
             src => string.IsNullOrEmpty(src.PictureUrl)
                    ? null
                    : $"{baseUrl}/{src.PictureUrl.TrimStart('/')}");
        }


    }
}