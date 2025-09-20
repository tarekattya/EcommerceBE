using Ecommerce.API.Mapping.ProductMap;
using Ecommerce.Application.Helper;
using Ecommerce.Application.Helper.Dtos.Product;
using Ecommerce.Application.Services.Service.Contarct;
using Ecommerce.Core.Entites;
using Ecommerce.Core.Entites.ProductModule;
using Ecommerce.Core.Specifications.ProductSpecs;
using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{

    public class ProductController(IProductService service) : ApiBaseController
    {
        private readonly IProductService _service = service;

        [HttpGet("")]
        public async Task<ActionResult<Pagination<productResponse>>> GetAllProducts([FromQuery] ProductSpecParams specParams)
        {
            var response = await _service.GetAllAsync(specParams);
            return response.IsSuccess ? Ok(response.Value) : NotFound(ProductErrors.InValidInputs);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<productResponse>> GetProductById([FromRoute] int id)
        {
            var response = await _service.GetProductById(id);
            return response.IsSuccess ? Ok(response.Value) : NotFound(ProductErrors.NotFoundProduct);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<ProductBrand>> GetBrands()
        {
            var brands = await _service.GetBrands();
            return brands.IsSuccess ? Ok(brands.Value) : NotFound(ProductErrors.NotFoundBrand);

        }

        [HttpGet("categories")]
        public async Task<ActionResult<ProductCategory>> GetCategories()
        {
            var brands = await _service.GetCategories();
            return brands.IsSuccess ? Ok(brands.Value) : NotFound(ProductErrors.NotFoundCate);

        }

    }
}
