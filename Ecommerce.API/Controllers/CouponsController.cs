namespace Ecommerce.API;

public class CouponsController(ICouponService service) : ApiBaseController
{
    private readonly ICouponService _service = service;

    [HttpGet]
    public async Task<ActionResult<Pagination<CouponResponse>>> GetAll([FromQuery] CouponSpecParams specParams)
    {
        var result = await _service.GetAllCouponsAsync(specParams);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<CouponResponse>> GetByCode(string code)
    {
        Result<CouponResponse>? result = await _service.GetCouponByCodeAsync(code);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize] 
    public async Task<ActionResult<CouponResponse>> Create(CouponRequest request)
    {
        Result<CouponResponse>? result = await _service.CreateCouponAsync(request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteCouponAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("validate/{code}")]
    public async Task<ActionResult<decimal>> Validate(string code, [FromQuery] decimal amount)
    {
        var result = await _service.ValidateAndApplyCouponAsync(code, amount);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
