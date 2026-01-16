namespace Ecommerce.Core;
public class OrderSpecification : BaseSpecifications<Order>
{
    public OrderSpecification(int id, string? email = null) 
        : base(o => o.Id == id && (string.IsNullOrEmpty(email) || o.BuyerEmail == email))
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);
    }

}
