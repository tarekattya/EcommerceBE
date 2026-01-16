namespace Ecommerce.Core;

public enum DiscountType
{
    Percentage,
    FixedAmount,
    FreeShipping,
    BOGO // Buy One Get One
}

public class Coupon : BaseEntity
{
    public Coupon() { }

    public Coupon(string code, decimal discountValue, DiscountType discountType, DateTime expiryDate, decimal? minimumAmount = null, int? usageLimit = null)
    {
        Code = code.ToUpper();
        DiscountValue = discountValue;
        DiscountType = discountType;
        ExpiryDate = expiryDate;
        MinimumAmount = minimumAmount;
        UsageLimit = usageLimit;
        UsageCount = 0;
        IsActive = true;
    }

    public string Code { get; set; } = default!;
    public decimal DiscountValue { get; set; }
    public DiscountType DiscountType { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal? MinimumAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }

    public bool IsValid()
    {
        if (!IsActive) return false;
        if (DateTime.UtcNow > ExpiryDate) return false;
        if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value) return false;
        return true;
    }

    public decimal CalculateDiscount(decimal amount, decimal shippingCost = 0, IEnumerable<OrderItem>? items = null)
    {
        if (MinimumAmount.HasValue && amount < MinimumAmount.Value) return 0;

        switch (DiscountType)
        {
            case DiscountType.Percentage:
                return amount * (DiscountValue / 100);
            
            case DiscountType.FixedAmount:
                return Math.Min(DiscountValue, amount);

            case DiscountType.FreeShipping:
                return shippingCost;

            case DiscountType.BOGO:
                if (items == null || !items.Any()) return 0;
                var totalQuantity = items.Sum(i => i.Quantity);
                if (totalQuantity < 2) return 0;
                
                return items.Min(i => i.Price);

            default:
                return 0;
        }
    }
}
