namespace Ecommerce.Core;

public class ProductVariantsByProductIdSpec : BaseSpecifications<ProductVariant>
{
    public ProductVariantsByProductIdSpec(int productId)
        : base(v => v.ProductId == productId)
    {
    }
}
