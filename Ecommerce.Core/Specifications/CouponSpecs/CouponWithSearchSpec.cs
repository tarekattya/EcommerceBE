namespace Ecommerce.Core;

public class CouponWithSearchSpec : BaseSpecifications<Coupon>
{
    public CouponWithSearchSpec(CouponSpecParams specParams) 
        : base(x => (string.IsNullOrEmpty(specParams.Search) || x.Code.ToLower().Contains(specParams.Search)) &&
                   (!specParams.Type.HasValue || x.DiscountType == specParams.Type))
    {
        ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
        
        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            switch (specParams.Sort)
            {
                case "expiryAsc":
                    AddOrderBy(x => x.ExpiryDate);
                    break;
                case "expiryDesc":
                    AddOrderByDesc(x => x.ExpiryDate);
                    break;
                case "valueAsc":
                    AddOrderBy(x => x.DiscountValue);
                    break;
                case "valueDesc":
                    AddOrderByDesc(x => x.DiscountValue);
                    break;
                default:
                    AddOrderByDesc(x => x.CreatedAt);
                    break;
            }
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
    }
}
