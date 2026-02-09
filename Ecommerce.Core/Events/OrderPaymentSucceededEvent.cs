namespace Ecommerce.Core;

public class OrderPaymentSucceededEvent(Order order) : BaseDomainEvent
{
    public Order Order { get; } = order;
}
