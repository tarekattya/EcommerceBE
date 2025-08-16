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
    }
}
