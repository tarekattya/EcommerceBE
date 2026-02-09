namespace Ecommerce.Core;

public class OrderCreatedEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}

