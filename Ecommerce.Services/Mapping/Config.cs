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
        }

    }
