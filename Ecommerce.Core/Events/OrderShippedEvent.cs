namespace Ecommerce.Core;

public class OrderShippedEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}
