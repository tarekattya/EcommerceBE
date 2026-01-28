namespace Ecommerce.Infrastructure;

internal class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Product)
            .WithMany()
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure a user can only have a product once in their wishlist
        builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        
        // Performance indexes
        builder.HasIndex(w => w.UserId);
        builder.HasIndex(w => w.IsDeleted);
    }
}
