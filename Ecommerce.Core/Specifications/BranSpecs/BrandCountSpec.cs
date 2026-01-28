namespace Ecommerce.Core;

public class BrandCountSpec : BaseSpecifications<ProductBrand>
{
    public BrandCountSpec(BrandSpecParams specParams) 
        : base(x => string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search))
    {
    }
}
