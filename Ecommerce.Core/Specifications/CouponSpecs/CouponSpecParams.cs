namespace Ecommerce.Core;

public record CouponSpecParams : BaseSpecParams
{
    public DiscountType? Type { get; init; }
}
