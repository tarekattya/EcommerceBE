namespace Ecommerce.Core;

public class OrderCancelledEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}

