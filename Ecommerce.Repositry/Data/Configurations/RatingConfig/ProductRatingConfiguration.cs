namespace Ecommerce.Infrastructure;

internal class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rating value constraint (1-5)
        builder.Property(r => r.Rating)
            .IsRequired();

        // Review is optional with max length
        builder.Property(r => r.Review)
            .HasMaxLength(2000);

        // Ensure a user can only rate a product once
        builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
        
        // Performance indexes
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.Rating);
        builder.HasIndex(r => r.IsDeleted);
    }
}
