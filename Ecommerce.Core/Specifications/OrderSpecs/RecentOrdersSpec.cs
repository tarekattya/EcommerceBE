
namespace Ecommerce.Core;

public class RecentOrdersSpec : BaseSpecifications<Order>
{
    public RecentOrdersSpec(int take) : base()
    {
        Includes.Add(e => e.DeliveryMethod);
        Includes.Add(e => e.Items);
        ApplyPagination(0, take);
    }
}
