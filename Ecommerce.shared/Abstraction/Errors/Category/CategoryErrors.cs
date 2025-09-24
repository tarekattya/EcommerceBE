using Ecommerce.Shared.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Abstraction.Errors.Category
{
    public static class CategoryErrors
    {
        public static Error CategoryNameAlreadyExists => new Error("CategoryNameExists", "A category with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);
        public static Error NotFoundCate => new Error("NotFoundCategories", "Categories not found", statusCode: 404);
        public static Error CateHasProducts => new Error("CateHasProducts", "Cannot delete category because products are linked to it", StatusCodes.Status409Conflict);
        public static Error CantCreateCategory => new Error("CantCreateCategory", "Cannot Create category now , try again", StatusCodes.Status400BadRequest);
    }
}
