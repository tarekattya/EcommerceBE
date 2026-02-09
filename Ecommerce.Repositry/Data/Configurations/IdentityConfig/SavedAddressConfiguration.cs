namespace Ecommerce.Infrastructure;

internal class SavedAddressConfiguration : IEntityTypeConfiguration<SavedAddress>
{
    public void Configure(EntityTypeBuilder<SavedAddress> builder)
    {
        builder.HasIndex(s => s.UserId);
    }
}
