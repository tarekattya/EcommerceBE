using Ecommerce.Shared.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Abstraction.Errors.Cart
{
    public static class CartErrors
    {
        public static Error NotFoundCart = new Error("NotFoundCart", "Not Found Cart Cart With this key", StatusCodes.Status404NotFound);
        public static Error CantCreateOrUpdate = new Error("CantCreateOrUpdate", "Cant Create Or Update this catr now , try later ", StatusCodes.Status405MethodNotAllowed);
    }
}
