using Ecommerce.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Configurations.ProductsConfig
{
    internal class ProductBrandConfuguration : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            builder.Property(B => B.Name)
               .IsRequired()
               .HasMaxLength(80);



        }
    }
}
