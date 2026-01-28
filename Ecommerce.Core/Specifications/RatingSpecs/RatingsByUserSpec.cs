namespace Ecommerce.Core;

public class RatingsByUserSpec : BaseSpecifications<ProductRating>
{
    public RatingsByUserSpec(string userId) : base(r => r.UserId == userId)
    {
        Includes.Add(e => e.Product);
        Includes.Add(e => e.Product.Brand);
        Includes.Add(e => e.Product.Category);
        AddOrderByDesc(r => r.CreatedAt);
    }
}
