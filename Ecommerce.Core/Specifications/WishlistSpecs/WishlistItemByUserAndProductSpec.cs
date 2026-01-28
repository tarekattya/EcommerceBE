namespace Ecommerce.Core;

public class WishlistItemByUserAndProductSpec : BaseSpecifications<WishlistItem>
{
    public WishlistItemByUserAndProductSpec(string userId, int productId) 
        : base(w => w.UserId == userId && w.ProductId == productId)
    {
            Includes.Add(e => e.Product);
            Includes.Add(e=>e.Product.Brand);
            Includes.Add(e=>e.Product.Category);
    }
}
