namespace Ecommerce.Core;

public class CategoryWithSearchSpec : BaseSpecifications<ProductCategory>
{
    public CategoryWithSearchSpec(CategorySpecParams specParams) 
        : base(x => string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search))
    {
        ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
        
        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            switch (specParams.Sort)
            {
                case "nameAsc":
                    AddOrderBy(x => x.Name);
                    break;
                case "nameDesc":
                    AddOrderByDesc(x => x.Name);
                    break;
                default:
                    AddOrderBy(x => x.Name);
                    break;
            }
        }
        else
        {
            AddOrderBy(x => x.Name);
        }
    }
}
