using Ecommerce.Core.Specifications.ProductSpecs;
using Ecommerce.Shared.Helper;
using Ecommerce.Shared.Helper.Dtos.Product;

namespace Ecommerce.API.Controllers
{

    public class ProductsController(IProductService service) : ApiBaseController
    {
        private readonly IProductService _service = service;

        [HttpGet("")]
        public async Task<ActionResult<Pagination<productResponse>>> GetAllProducts([FromQuery] ProductSpecParams specParams)
        {
            var products = await _service.GetAllAsync(specParams);
            return products.IsSuccess ? Ok(products.Value) : products.ToProblem();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<productResponse>> GetProductById([FromRoute] int id)
        {
            var product = await _service.GetProductById(id);
            return product.IsSuccess ? Ok(product.Value) : product.ToProblem();
        }

        [HttpPost("")]
        public async Task<ActionResult<productResponse>> CreateProduct([FromQuery] ProductRequest request)
        {
            var product = await _service.CreateProduct(request);
            return product.IsSuccess ? Ok(product.Value) : product.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<productResponse>> UpdateProduct([FromRoute] int id, [FromQuery] ProductRequest request)
        {
            var product = await _service.UpdateProduct(id, request);
            return product.IsSuccess ? NoContent() : product.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _service.DeleteProduct(id);
            return product.IsSuccess ? NoContent() : product.ToProblem();

        }

    }
}