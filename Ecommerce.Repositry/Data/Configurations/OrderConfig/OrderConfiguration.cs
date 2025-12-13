
namespace Ecommerce.Infrastructure;
internal class OrderConfiguration : IEntityTypeConfiguration<Core.Order>
{
    public void Configure(EntityTypeBuilder<Core.Order> builder)
    {
        builder.OwnsOne(e => e.ShipingAddress, e => e.WithOwner());
        builder.Property(e => e.Status).HasConversion(e => e.ToString(), e => (OrderStatus)Enum.Parse(typeof(OrderStatus), e));
        builder.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");

        builder.HasOne(e => e.DeliveryMethod)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

    }
}
