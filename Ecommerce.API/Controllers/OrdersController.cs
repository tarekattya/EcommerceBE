namespace Ecommerce.API;

public class OrdersController(IOrderService orderService) : ApiBaseController
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
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
        string? userEmail = User.FindFirstValue(ClaimTypes.Email)!;

        var result = await _orderService.CancelOrderAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request)
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email)!;
        Result<OrderResponse>? result = await _orderService.UpdateOrderStatusAsync(id, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetOrdersForUserAsync()
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email);
        
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized();

        Result<IReadOnlyList<OrderResponse>>? result = await _orderService.GetOrdersForUserAsync(userEmail);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
