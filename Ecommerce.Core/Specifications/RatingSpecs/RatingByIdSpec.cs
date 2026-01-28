namespace Ecommerce.Core;

public class RatingByIdSpec : BaseSpecifications<ProductRating>
{
    public RatingByIdSpec(int ratingId) : base(r => r.Id == ratingId)
    {
        Includes.Add(e => e.Product);
        Includes.Add(e => e.Product.Brand);
        Includes.Add(e => e.Product.Category);
    }
}
