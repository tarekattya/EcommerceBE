using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.Identity;
using Ecommerce.Core.Entites.ProductModule;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):IdentityDbContext<ApplicationUser>(options)
    {


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ProductBrand> ProductBrands { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;



    }
}
