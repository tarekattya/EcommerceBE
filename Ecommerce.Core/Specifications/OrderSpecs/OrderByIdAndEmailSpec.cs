namespace Ecommerce.Core;

public class OrderByIdAndEmailSpec : BaseSpecifications<Order>
{
    public OrderByIdAndEmailSpec(int id, string email) 
        : base(o => o.Id == id && o.BuyerEmail == email)
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);
    }
}
