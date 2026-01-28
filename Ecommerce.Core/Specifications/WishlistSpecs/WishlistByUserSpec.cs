namespace Ecommerce.Core;

public class WishlistByUserSpec : BaseSpecifications<WishlistItem>
{
    public WishlistByUserSpec(string userId) : base(w => w.UserId == userId)
    {
        Includes.Add(e => e.Product);
        Includes.Add(e => e.Product.Brand);
        Includes.Add(e => e.Product.Category);
        AddOrderByDesc(w => w.CreatedAt);
    }
}
