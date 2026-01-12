namespace Ecommerce.Core;

public class OrderProcessingStartedEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}
