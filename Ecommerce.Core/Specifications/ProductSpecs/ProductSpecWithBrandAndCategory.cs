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


        public ProductSpecWithBrandAndCategory(string? sort, int? brandId, int? categoryId) : base(
            p => (!brandId.HasValue || p.BrandId == brandId) &&
            (!categoryId.HasValue || p.CategoryId == categoryId))
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {

                    case "PriceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name); break;


                }
            }
            else 
                AddOrderBy(p => p.Name);
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
}
