namespace Ecommerce.Core;

public class DeliveryMethod : BaseEntity
{
    public DeliveryMethod()
    {
    }

    public DeliveryMethod(string shortName, string description, decimal cost, string deliveryTime)
    {
        ShortName = shortName;
        Description = description;
        Cost = cost;
        DeliveryTime = deliveryTime;
    }

    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string DeliveryTime { get; set; } = string.Empty;

}
