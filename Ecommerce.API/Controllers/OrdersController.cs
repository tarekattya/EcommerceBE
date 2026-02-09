namespace Ecommerce.API;

public class OrdersController(IOrderService orderService) : ApiBaseController
{
    private readonly IOrderService _orderService = orderService;



    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();

        var result = await _orderService.GetOrderByIdAsync(id, UserEmail);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost()]
    public async Task<IActionResult> CreateOrderAsync(OrderRequest request , CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateOrderAsync(request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrderAsync(int id)
    {
        if (string.IsNullOrEmpty(UserEmail)) return Unauthorized();
        var result = await _orderService.CancelOrderAsync(id, UserEmail);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request)
    {
        Result<OrderResponse>? result = await _orderService.UpdateOrderStatusAsync(id, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetOrdersForUserAsync([FromQuery] OrderSpecParams specParams)
    {
        if (string.IsNullOrEmpty(UserEmail))
            return Unauthorized();

        var result = await _orderService.GetOrdersForUserAsync(UserEmail, specParams);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>Track order by tracking number (e.g. carrier number). No auth required.</summary>
    [AllowAnonymous]
    [HttpGet("track")]
    public async Task<IActionResult> GetOrderByTrackingNumberAsync([FromQuery] string trackingNumber)
    {
        var result = await _orderService.GetOrderByTrackingNumberAsync(trackingNumber);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>Get invoice for an order (buyer: own orders only; Admin: any order).</summary>
    [Authorize]
    [HttpGet("{id}/invoice")]
    public async Task<IActionResult> GetInvoiceAsync(int id)
    {
        if (string.IsNullOrEmpty(UserEmail) && !User.IsInRole("Admin")) return Unauthorized();
        var email = User.IsInRole("Admin") ? null : UserEmail;
        var result = await _orderService.GetInvoiceAsync(id, email);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
