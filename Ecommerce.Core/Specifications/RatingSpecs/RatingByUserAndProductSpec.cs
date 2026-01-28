namespace Ecommerce.Core;

public class RatingByUserAndProductSpec : BaseSpecifications<ProductRating>
{
    public RatingByUserAndProductSpec(string userId, int productId) 
        : base(r => r.UserId == userId && r.ProductId == productId)
    {
        Includes.Add(e => e.Product);
        Includes.Add(e => e.Product.Brand);
        Includes.Add(e => e.Product.Category);
    }
}
