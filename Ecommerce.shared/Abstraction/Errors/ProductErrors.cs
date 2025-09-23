using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Abstraction.Errors
{
    public static class ProductErrors
    {
        public static Error NotFoundProduct => new Error("NotFoundProduct" , "Product with this id not found" , statusCode:404);
        public static Error NotFoundBrand => new Error("NotFoundBrands" , "brands not found" , statusCode:404);
        public static Error NotFoundCate => new Error("NotFoundCategories", "Categories not found" , statusCode:404);

        public static Error InValidInputs => new Error("Invalid Inputs", "this inputs not match any product not found", statusCode: 404);
        public static Error BrandHasProducts => new Error("BrandHasProducts" , "Cannot delete brand because products are linked to it", StatusCodes.Status409Conflict);
        public static Error CateHasProducts => new Error("CateHasProducts", "Cannot delete category because products are linked to it", StatusCodes.Status409Conflict);
        public static Error CantCreateBrand => new Error("CantCreateBrand", "Cannot Create brand now , try again", StatusCodes.Status400BadRequest);
        public static Error CantCreateCategory => new Error("CantCreateCategory", "Cannot Create category now , try again", StatusCodes.Status400BadRequest);

        public static Error ProductNameAlreadyExists => new Error("ProductNameExists","A product with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);

        public static Error BrandNameAlreadyExists => new Error( "BrandNameExists", "A brand with this name already exists. Please choose a different name.",StatusCodes.Status409Conflict);

        public static Error CategoryNameAlreadyExists => new Error("CategoryNameExists", "A category with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);

  









    }
}
