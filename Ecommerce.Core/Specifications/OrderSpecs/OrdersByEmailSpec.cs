namespace Ecommerce.Core;

public class OrdersByEmailSpec : BaseSpecifications<Order>
{
    public OrdersByEmailSpec(string email) 
        : base(o => o.BuyerEmail == email)
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);
        AddOrderByDesc(o => o.OrderDate);
    }
}
