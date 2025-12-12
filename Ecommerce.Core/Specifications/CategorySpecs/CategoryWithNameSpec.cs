using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core;

    public class CategoryWithNameSpec : BaseSpecifications<ProductCategory> 
    {
        public CategoryWithNameSpec(string Name) : base(p => p.Name == Name)
        {
            
        }
    }

