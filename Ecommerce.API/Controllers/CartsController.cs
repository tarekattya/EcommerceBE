namespace Ecommerce.API;

public class CartsController(ICartService cartService) : ApiBaseController
{
    private readonly ICartService _cartService = cartService;

    [HttpGet("{id}")]
    public async Task<ActionResult<CartResponse>> GetCart([FromRoute]string id)
    {
        var result = await _cartService.GetCartAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPost()]
    public async Task<ActionResult<CartResponse>> Create([FromBody]CartRequest cartRequest)
    {
        var result = await _cartService.CreateCartAsync(cartRequest);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPut("")]
    public async Task<ActionResult<CartResponse>> Update([FromBody]CartRequest cartRequest)
    {
        var result = await _cartService.UpdateCartAsync(cartRequest);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPut("items/remove")]
    public async Task<ActionResult<CartResponse>> RemoveItems([FromBody]RemoveCartItemsRequest cartRequest)
    {
        var result = await _cartService.RemoveItemsAsync(cartRequest);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [HttpPut("items/Add")]
    public async Task<ActionResult<CartResponse>> AddItems([FromBody] AddCartItemsRequest cartRequest)
    {
        var result = await _cartService.AddItemsAsync(cartRequest);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpDelete()]
    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await _cartService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


}
