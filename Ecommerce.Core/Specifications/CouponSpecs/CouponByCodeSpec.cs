namespace Ecommerce.Core;

public class CouponByCodeSpec : BaseSpecifications<Coupon>
{
    public CouponByCodeSpec(string code) : base(c => c.Code == code.ToUpper())
    {
    }
}
