namespace Ecommerce.Core;

public class OrderDeliveredEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}
