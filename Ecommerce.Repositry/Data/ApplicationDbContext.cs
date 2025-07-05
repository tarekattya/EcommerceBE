using Ecommerce.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
    {


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ProductBrand> ProductBrands { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;



    }
}
