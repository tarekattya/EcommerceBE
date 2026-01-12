using System.Collections.Generic;

namespace Ecommerce.Core;

public class OrderStockReleaseEvent(IEnumerable<OrderItem> items) : BaseDomainEvent
{
    public IEnumerable<OrderItem> Items { get; } = items;
}
