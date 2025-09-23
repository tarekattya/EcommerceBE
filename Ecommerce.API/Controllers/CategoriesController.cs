using Ecommerce.API.Abstractions;
using Ecommerce.Shared.Helper.Dtos.Category;
using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Shared.Abstraction;
using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Services.Service.Contarct;

namespace Ecommerce.API.Controllers
{
    public class CategoriesController(ICategoryService service) : ApiBaseController
    {
        private readonly ICategoryService _service = service;

        [HttpGet("")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _service.GetCategories();
            return categories.IsSuccess ? Ok(categories.Value) : categories.ToProblem();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            var category = await _service.GetCategoryById(id);
            return category.IsSuccess ? Ok(category.Value) : category.ToProblem();
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateCategory([FromQuery] CategoryRequest request)
        {
            var category = await _service.CreateCategory(request);
            return category.IsSuccess ? Ok(category.Value) : category.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromQuery] CategoryRequest request)
        {
            var category = await _service.UpdateCategory(id, request);
            return category.IsSuccess ? NoContent() : category.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _service.DeleteCategory(id);
            return category.IsSuccess ? Ok() : category.ToProblem();
        }
    }
}
