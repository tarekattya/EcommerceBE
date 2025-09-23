using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.BranSpecs
{
    public class BrandsByNameSpec : BaseSpecifications<ProductBrand>
    {
        public BrandsByNameSpec(string Name) : base(p => p.Name == Name)
        {
        }
    }
}
