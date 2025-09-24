using Ecommerce.Shared.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Abstraction.Errors.Product
{
    public static class ProductErrors
    {
        public static Error NotFoundProduct => new Error("NotFoundProduct" , "Product with this id not found" , statusCode:404);
        public static Error ProductNameAlreadyExists => new Error("ProductNameExists","A product with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);
        public static Error InValidInputs => new Error("Invalid Inputs", "this inputs not match any product not found", statusCode: 404);


 







    }
}
