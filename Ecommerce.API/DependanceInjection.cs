using Ecommerce.API.Data;
using Ecommerce.Core.RepositoryContracts;
using Ecommerce.Repositry;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API
{
    public static class DependanceInjection
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services , IConfiguration configuration)
        {
          
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
