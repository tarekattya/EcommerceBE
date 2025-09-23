using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    public class BrandsController(IBrandService service) : ApiBaseController
    {
        private readonly IBrandService _service = service;

        [HttpGet("")]
        public async Task<ActionResult<ProductBrand>> GetBrands()
        {
            var brands = await _service.GetBrands();
            return brands.IsSuccess ? Ok(brands.Value) : NotFound(ProductErrors.NotFoundBrand);

        }

    }
}
