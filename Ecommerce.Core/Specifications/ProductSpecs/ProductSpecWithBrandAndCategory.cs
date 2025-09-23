using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecommerce.Core.Specifications.ProductSpecs
{
    public class ProductSpecWithBrandAndCategory : BaseSpecifications<Product>
    {


        public ProductSpecWithBrandAndCategory(ProductSpecParams specParams) : base(

            p =>
           (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
            (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId) &&
            (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId))
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
                    default:
                        AddOrderBy(p => p.Name); break;


                }
            }
            else 
                AddOrderBy(p => p.Name);

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
}
