using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public class ProductsByCateSpec :BaseSpecifications<Product>
    {
        public ProductsByCateSpec(int CateId): base(p => p.Id == CateId)
        {
                
        }
    }
}
