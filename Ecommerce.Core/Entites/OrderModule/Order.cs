namespace Ecommerce.Core;
public class Order : BaseEntity
{
    public Order(string buyerEmail, OrderAddress shipingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
    {
        BuyerEmail = buyerEmail;
        ShipingAddress = shipingAddress;
        DeliveryMethod = deliveryMethod;
        Items = items;
        SubTotal = subTotal;
    }

    public Order()
    {
    }

    public string BuyerEmail { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public OrderAddress ShipingAddress { get; set; } = default!;

    public DeliveryMethod DeliveryMethod { get; set; } = default!;

    ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();

    public decimal SubTotal { get; set; }

    public decimal GetTotal() =>
        SubTotal + DeliveryMethod.Cost;

}
