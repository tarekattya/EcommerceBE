namespace Ecommerce.API;

[Authorize]
public class SavedAddressesController(ISavedAddressService savedAddressService) : ApiBaseController
{
    private readonly ISavedAddressService _savedAddressService = savedAddressService;

    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        if (string.IsNullOrEmpty(UserId)) return Unauthorized();
        var result = await _savedAddressService.GetUserAddressesAsync(UserId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Add(SavedAddressRequest request)
    {
        if (string.IsNullOrEmpty(UserId)) return Unauthorized();
        var result = await _savedAddressService.AddAsync(UserId, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SavedAddressRequest request)
    {
        if (string.IsNullOrEmpty(UserId)) return Unauthorized();
        var result = await _savedAddressService.UpdateAsync(id, UserId, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (string.IsNullOrEmpty(UserId)) return Unauthorized();
        var result = await _savedAddressService.DeleteAsync(id, UserId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/set-default")]
    public async Task<IActionResult> SetDefault(int id)
    {
        if (string.IsNullOrEmpty(UserId)) return Unauthorized();
        var result = await _savedAddressService.SetDefaultAsync(id, UserId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
