using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Abstraction.Errors
{
    public static class ProductErrors
    {
        public static Error NotFoundProduct => new Error("NotFoundProduct" , "Product with this id not found" , statusCode:404);
        public static Error NotFoundBrand => new Error("NotFoundBrands" , "brands not found" , statusCode:404);
        public static Error NotFoundCate => new Error("NotFoundCategories", "Categories not found" , statusCode:404);

        public static Error InValidInputs => new Error("Invalid Inputs", "this inputs not match any product not found", statusCode: 404);



    }
}
