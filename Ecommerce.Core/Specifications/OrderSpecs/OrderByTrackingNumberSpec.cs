namespace Ecommerce.Core;

public class OrderByTrackingNumberSpec : BaseSpecifications<Order>
{
    public OrderByTrackingNumberSpec(string trackingNumber)
        : base(o => o.TrackingNumber != null && o.TrackingNumber == trackingNumber.Trim())
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);
    }
}
