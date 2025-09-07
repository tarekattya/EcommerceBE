using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Core.Abstraction;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Data;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.Arm;

namespace Ecommerce.API
{
    public static class DependanceInjection
    {

       
        public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration configuration,WebApplicationBuilder webApplication)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMapping(configuration);
            services.AddValidtionResponse(webApplication);


            services.AddDBServices(configuration);
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }

        public static IServiceCollection AddDBServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options/*.UseLazyLoadingProxies()*/.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });
            return services;
        }
        public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<BaseUrl>(configuration.GetSection("BaseUrl"));

            services.AddSingleton<IMapper>(sp =>
            {
                var urlOptions = sp.GetRequiredService<IOptions<BaseUrl>>();

                var config = TypeAdapterConfig.GlobalSettings;
                Config.Register(config, urlOptions);

                return new Mapper(config);
            });

            return services;
        }

        public static IServiceCollection AddValidtionResponse(this IServiceCollection services , WebApplicationBuilder webApplication)
        {
            webApplication.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage).ToList();

                    var errorResponse = new ApiValidationResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}
