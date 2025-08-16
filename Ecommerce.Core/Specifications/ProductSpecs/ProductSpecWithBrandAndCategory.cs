using Ecommerce.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public class ProductSpecWithBrandAndCategory : BaseSpecifications<Product>
    {


        public ProductSpecWithBrandAndCategory() : base()
        {
            AddIncludes();
        }


        public ProductSpecWithBrandAndCategory(int id) : base(p=>p.Id == id)
        {
            AddIncludes();
        }
        private void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }



    }
}
