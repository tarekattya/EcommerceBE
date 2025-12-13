
namespace Ecommerce.Infrastructure;
internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(e => e.ProductItemOrderd , e => e.WithOwner());
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
    }
}

