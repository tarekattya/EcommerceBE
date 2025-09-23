
namespace Ecommerce.Infrastructure.Configurations.ProductsConfig
{
    internal class ProductCategoryConfuguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.Property(C => C.Name)
                .IsRequired()
                .HasMaxLength(80);
                       

        }
    }
}
