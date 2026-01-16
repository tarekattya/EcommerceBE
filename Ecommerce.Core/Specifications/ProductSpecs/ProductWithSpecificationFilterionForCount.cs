namespace Ecommerce.Core;

    public class ProductWithSpecificationFilterionForCount : BaseSpecifications<Product>
    {
        public ProductWithSpecificationFilterionForCount(ProductSpecParams specParams) :base( p => 
                (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search) || p.Description.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || specParams.BrandId == p.BrandId) &&
                (!specParams.CategoryId.HasValue || specParams.CategoryId == p.CategoryId) &&
                (!specParams.MinPrice.HasValue || p.Price >= specParams.MinPrice) &&
                (!specParams.MaxPrice.HasValue || p.Price <= specParams.MaxPrice)
            )
        {}
    }
