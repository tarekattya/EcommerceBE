using Ecommerce.Shared;

namespace Ecommerce.API;

[Route("api/products/{productId:int}/variants")]
[ApiController]
public class ProductVariantsController(IProductVariantService variantService) : ControllerBase
{
    private readonly IProductVariantService _variantService = variantService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductVariantResponse>>> GetVariants([FromRoute] int productId, CancellationToken cancellationToken)
    {
        var list = await _variantService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(list);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductVariantResponse>> CreateVariant([FromRoute] int productId, [FromBody] ProductVariantRequest request, CancellationToken cancellationToken)
    {
        var result = await _variantService.CreateAsync(productId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{variantId:int}")]
    public async Task<ActionResult<ProductVariantResponse>> UpdateVariant([FromRoute] int productId, [FromRoute] int variantId, [FromBody] ProductVariantRequest request, CancellationToken cancellationToken)
    {
        var result = await _variantService.UpdateAsync(variantId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{variantId:int}")]
    public async Task<ActionResult> DeleteVariant([FromRoute] int productId, [FromRoute] int variantId, CancellationToken cancellationToken)
    {
        var result = await _variantService.DeleteAsync(variantId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
