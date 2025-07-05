using Ecommerce.Infrastructure;
using Ecommerce.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.API
{
    public static class DependanceInjection
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
    }
}
