namespace Ecommerce.Core;

public class BrandsByNameSpec : BaseSpecifications<ProductBrand>
{
    public BrandsByNameSpec(string Name) : base(p => p.Name == Name)
    {
    }
}

