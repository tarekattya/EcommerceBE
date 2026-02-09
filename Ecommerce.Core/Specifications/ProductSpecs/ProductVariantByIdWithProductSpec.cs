namespace Ecommerce.Core;

public class ProductVariantByIdWithProductSpec : BaseSpecifications<ProductVariant>
{
    public ProductVariantByIdWithProductSpec(int id)
        : base(v => v.Id == id)
    {
        Includes.Add(v => v.Product);
        IncludeStrings.Add("Product.Brand");
        IncludeStrings.Add("Product.Category");
    }
}
