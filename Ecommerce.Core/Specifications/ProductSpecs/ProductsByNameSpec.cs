using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public class ProductsByNameSpec : BaseSpecifications<Product>
    {
        public ProductsByNameSpec(string Name) : base(p => p.Name == Name)
        {
        }
    }
    
    
}
