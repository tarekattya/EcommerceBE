namespace Ecommerce.Core;

public class CategoryCountSpec : BaseSpecifications<ProductCategory>
{
    public CategoryCountSpec(CategorySpecParams specParams) 
        : base(x => string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search))
    {
    }
}
