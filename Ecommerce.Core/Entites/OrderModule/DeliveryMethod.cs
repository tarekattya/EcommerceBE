namespace Ecommerce.Core;

public class DeliveryMethod : AuditLogging
{
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string DeliveryTime { get; set; } = string.Empty;

}
