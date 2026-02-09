namespace Ecommerce.Infrastructure;

internal class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.Property(p => p.Size).HasMaxLength(50);
        builder.Property(p => p.Color).HasMaxLength(50);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.ProductId, p.Size, p.Color }).IsUnique();
        builder.HasIndex(p => p.SKU).IsUnique();
    }
}
