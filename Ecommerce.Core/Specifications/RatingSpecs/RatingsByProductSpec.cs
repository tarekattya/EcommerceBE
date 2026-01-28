namespace Ecommerce.Core;

public class RatingsByProductSpec : BaseSpecifications<ProductRating>
{
    public RatingsByProductSpec(int productId) : base(r => r.ProductId == productId)
    {
        Includes.Add(e => e.Product);
        Includes.Add(e => e.Product.Brand);
        Includes.Add(e => e.Product.Category);
        AddOrderByDesc(r => r.CreatedAt);
    }
}
