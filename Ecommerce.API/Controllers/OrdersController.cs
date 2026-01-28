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
        var result = await _orderService.CancelOrderAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

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

}
