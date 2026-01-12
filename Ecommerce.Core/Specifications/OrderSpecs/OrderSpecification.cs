namespace Ecommerce.Core;
public class OrderSpecification : BaseSpecifications<Order>
{
    public OrderSpecification(int id) 
        : base(o => o.Id == id)
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);
    }

}
