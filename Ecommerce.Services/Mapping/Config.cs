using System.Reflection;
using System.Runtime.Serialization;
using Ecommerce.Shared;

namespace Ecommerce.Application;

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

        TypeAdapterConfig<Product, CartItem>.NewConfig()
     .Map(dest => dest.ProductName, src => src.Name)
     .Map(dest => dest.Brand, src => src.Brand.Name)
     .Map(dest => dest.Type, src => src.Category.Name)
     .Map(dest => dest.PictureUrl,
          src => string.IsNullOrEmpty(src.PictureUrl)
                 ? null
                 : $"{baseUrl}/{src.PictureUrl.TrimStart('/')}");
        TypeAdapterConfig<OrderAddressRequest, OrderAddress>.NewConfig();
        TypeAdapterConfig<Order, OrderResponse>.NewConfig()
            .Map(dest => dest.OrderAddress, src => src.ShipingAddress)
            .Map(dest => dest.DeliveryMethodName, src => src.DeliveryMethod.ShortName)
            .Map(dest => dest.Items, src => src.Items);
        TypeAdapterConfig<OrderItem, OrderItemResponse>.NewConfig()
            .Map(dest => dest.ProductId, src => src.ProductItemOrderd.ProductId)
            .Map(dest => dest.ProductName, src => src.ProductItemOrderd.Name)
            .Map(dest => dest.PictureUrl, src => src.ProductItemOrderd.PictureUrl);
    }

}
