namespace Ecommerce.API;

[Authorize]
public class AddressController(UserManager<ApplicationUser> userManager , ApplicationDbContext context) : ApiBaseController
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    [HttpGet("")]
    public async Task<ActionResult> GetUserAddress(CancellationToken cancellationToken)
    {
        var result = await _userManager.GetUserAddressAsync(User, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("")]
    public async Task<ActionResult<AddressResponse>> UpdateUserAddress(AddressRequest request, CancellationToken cancellationToken)
    {
      var user = await _userManager.GetUserAsync(User, cancellationToken);
        if (user == null) return NotFound();
        var newAddress = request.Adapt<Address>();
        if (user.Address is not null)
        {
            newAddress.Id = user.Address.Id;
            user.Address = newAddress;
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(request.Adapt<AddressResponse>());
        }
        return BadRequest();
    }


}
