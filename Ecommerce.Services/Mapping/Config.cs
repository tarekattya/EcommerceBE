using System.Reflection;
using System.Runtime.Serialization;
using Ecommerce.Shared;

namespace Ecommerce.Application;

public class Config
{
    private static string GetEnumMemberValue(Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null) return enumValue.ToString();
        
        var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
        return attribute?.Value ?? enumValue.ToString();
    }

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
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.PictureUrl,
                 src => string.IsNullOrEmpty(src.PictureUrl)
                        ? null
                        : $"{baseUrl}/{src.PictureUrl.TrimStart('/')}");
        TypeAdapterConfig<OrderAddressRequest, OrderAddress>.NewConfig();
        TypeAdapterConfig<Order, OrderResponse>.NewConfig()
            .Map(dest => dest.OrderAddress, src => src.ShipingAddress)
            .Map(dest => dest.DeliveryMethodName, src => src.DeliveryMethod.ShortName)
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.OrderStatus, src => GetEnumMemberValue(src.Status))
            .Map(dest => dest.Total, src => src.GetTotal())
            .Map(dest => dest.CouponCode, src => src.CouponCode)
            .Map(dest => dest.Discount, src => src.Discount);

        config.NewConfig<Coupon, CouponResponse>()
            .Map(dest => dest.DiscountType, src => (DiscountTypeDto)src.DiscountType);

        TypeAdapterConfig<OrderItem, OrderItemResponse>.NewConfig()
            .Map(dest => dest.ProductId, src => src.ProductItemOrderd.ProductId)
            .Map(dest => dest.ProductVariantId, src => src.ProductItemOrderd.ProductVariantId)
            .Map(dest => dest.Size, src => src.ProductItemOrderd.Size)
            .Map(dest => dest.Color, src => src.ProductItemOrderd.Color)
            .Map(dest => dest.ProductName, src => src.ProductItemOrderd.Name)
            .Map(dest => dest.PictureUrl, src => src.ProductItemOrderd.PictureUrl);

        // Wishlist mappings
        config.NewConfig<WishlistItem, WishlistItemResponse>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.PictureUrl, 
                 src => string.IsNullOrEmpty(src.Product.PictureUrl)
                        ? null
                        : $"{baseUrl}/{src.Product.PictureUrl.TrimStart('/')}")
            .Map(dest => dest.Price, src => src.Product.Price)
            .Map(dest => dest.Brand, src => src.Product.Brand.Name)
            .Map(dest => dest.Category, src => src.Product.Category.Name)
            .Map(dest => dest.AddedAt, src => src.CreatedAt);

        // Rating mappings
        config.NewConfig<ProductRating, RatingResponse>()
            .Map(dest => dest.ProductName, src => src.Product.Name);
    }

}
