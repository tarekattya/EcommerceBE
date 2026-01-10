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


}
