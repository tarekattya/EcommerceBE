
namespace Ecommerce.Core;

    public class ProductsByBrandSpec : BaseSpecifications<Product>
    {
        public ProductsByBrandSpec(int brandId) : base(p => p.BrandId == brandId)
        {
        }
    }

