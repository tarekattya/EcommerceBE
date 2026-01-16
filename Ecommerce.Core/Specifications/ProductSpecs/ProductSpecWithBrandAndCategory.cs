
namespace Ecommerce.Core;

    public class ProductSpecWithBrandAndCategory : BaseSpecifications<Product>
    {
    public ProductSpecWithBrandAndCategory(ProductSpecParams specParams) : base(
            p =>
            (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search) || p.Description.ToLower().Contains(specParams.Search)) &&
            (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId) &&
            (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId) &&
            (!specParams.MinPrice.HasValue || p.Price >= specParams.MinPrice) &&
            (!specParams.MaxPrice.HasValue || p.Price <= specParams.MaxPrice))
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    case "DateAsc":
                        AddOrderBy(p => p.CreatedAt);
                        break;
                    case "DateDesc":
                        AddOrderByDesc(p => p.CreatedAt);
                        break;
                    default:
                        AddOrderBy(p => p.Name); break;
                }
            }
            else 
                AddOrderByDesc(p => p.CreatedAt);

            ApplyPagination((specParams.pageIndex - 1) * specParams.PageSize, specParams.PageSize);
        }
        public ProductSpecWithBrandAndCategory(int id) : base(p => p.Id == id)
        {
            AddIncludes();
        }
        private void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }

