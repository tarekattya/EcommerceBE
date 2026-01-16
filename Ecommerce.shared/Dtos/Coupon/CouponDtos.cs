namespace Ecommerce.Shared;

public enum DiscountTypeDto
{
    Percentage,
    FixedAmount,
    FreeShipping,
    BOGO
}

public record CouponRequest(
    string Code,
    decimal DiscountValue,
    DiscountTypeDto DiscountType,
    DateTime ExpiryDate,
    decimal? MinimumAmount,
    int? UsageLimit
);

public record CouponResponse(
    int Id,
    string Code,
    decimal DiscountValue,
    DiscountTypeDto DiscountType,
    DateTime ExpiryDate,
    decimal? MinimumAmount,
    int? UsageLimit,
    int UsageCount,
    bool IsActive
);
