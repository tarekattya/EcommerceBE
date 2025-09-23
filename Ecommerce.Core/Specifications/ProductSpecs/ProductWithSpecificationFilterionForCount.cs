using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public class ProductWithSpecificationFilterionForCount : BaseSpecifications<Product>

    {
        public ProductWithSpecificationFilterionForCount(ProductSpecParams specParams) :base( p => 


                (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || specParams.BrandId == p.BrandId) &&
                (!specParams.CategoryId.HasValue || specParams.CategoryId == p.CategoryId)






            )
        {
            
        }
    }
}
