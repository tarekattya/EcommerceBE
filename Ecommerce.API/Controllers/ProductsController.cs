
using Ecommerce.Shared;

namespace Ecommerce.API;
public class ProductsController(IProductService service) : ApiBaseController
{
    private readonly IProductService _service = service;

    [Cached(600)]
    [HttpGet("")]
    public async Task<ActionResult<Pagination<productResponse>>> GetAllProducts([FromQuery] ProductSpecParams specParams)
    {
        Result<Pagination<productResponse>>? products = await _service.GetAllAsync(specParams);
        return products.IsSuccess ? Ok(products.Value) : products.ToProblem();
    }


    [Cached(600)]
    [HttpGet("{id}")]
    public async Task<ActionResult<productResponse>> GetProductById([FromRoute] int id)
    {
        Result<productResponse>? product = await _service.GetProductById(id);
        return product.IsSuccess ? Ok(product.Value) : product.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("")]
    public async Task<ActionResult<productResponse>> CreateProduct([FromForm] ProductRequest request, IFormFile? image)
    {
        Result<productResponse>? product = await _service.CreateProduct(request, image);
        return product.IsSuccess ? Ok(product.Value) : product.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<productResponse>> UpdateProduct([FromRoute] int id, [FromForm] ProductRequest request, IFormFile? image)
    {
        Result<productResponse>? product = await _service.UpdateProduct(id, request, image);
        return product.IsSuccess ? NoContent() : product.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] int id)
    {
        Result? product = await _service.DeleteProduct(id);
        return product.IsSuccess ? NoContent() : product.ToProblem();
    }

    [Cached(3600)]
    [HttpGet("filters")]
    public async Task<ActionResult<ProductFiltersResponse>> GetProductFilters()
    {
        Result<ProductFiltersResponse>? result = await _service.GetProductFiltersAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}