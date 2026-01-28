namespace Ecommerce.Core;

public class CouponCountSpec : BaseSpecifications<Coupon>
{
    public CouponCountSpec(CouponSpecParams specParams) 
        : base(x => (string.IsNullOrEmpty(specParams.Search) || x.Code.ToLower().Contains(specParams.Search)) &&
                   (!specParams.Type.HasValue || x.DiscountType == specParams.Type))
    {
    }
}
