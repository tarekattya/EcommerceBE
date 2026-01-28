

namespace Ecommerce.Core;

public class OrderWithItemsSpec : BaseSpecifications<Order>
{
    public OrderWithItemsSpec() : base(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.PaymentFailed)
    {
        Includes.Add(o => o.Items);
    }
}
