using Ecommerce.Shared.Helper.Dtos.Category;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Services.Service.Contarct
{
    public interface ICategoryService
    {
        public Task<Result<IReadOnlyList<CategoryResponse>>> GetCategories();

        public Task<Result<CategoryResponse>> GetCategoryById(int id);

        public Task<Result<CategoryResponse>> CreateCategory(CategoryRequest category);

        public Task<Result> UpdateCategory(int id, CategoryRequest category);

        public Task<Result> DeleteCategory(int id);



    }
}
