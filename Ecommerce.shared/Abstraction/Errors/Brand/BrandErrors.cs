﻿using Ecommerce.Shared.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Abstraction.Errors.Brand
{
    public static class BrandErrors
    {
        public static Error NotFoundBrand => new Error("NotFoundBrands", "brands not found", statusCode: 404);
        public static Error BrandHasProducts => new Error("BrandHasProducts", "Cannot delete brand because products are linked to it", StatusCodes.Status409Conflict);
        public static Error BrandNameAlreadyExists => new Error("BrandNameExists", "A brand with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);
        public static Error CantCreateBrand => new Error("CantCreateBrand", "Cannot Create brand now , try again", StatusCodes.Status400BadRequest);
    }
}
