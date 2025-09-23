using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    public class CategoriesController(ICateGoryService service) : ApiBaseController
    {
        private readonly ICateGoryService _service = service;

        [HttpGet("")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _service.GetCategories();
            return categories.IsSuccess ? Ok(categories.Value) : NotFound(ProductErrors.NotFoundCate);
        }
    }
}
