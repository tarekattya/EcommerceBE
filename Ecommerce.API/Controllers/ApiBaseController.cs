

namespace Ecommerce.API;

[Route("api/[controller]")]
[ApiController]
public class ApiBaseController : ControllerBase
{
    protected string? UserEmail => User.FindFirstValue(ClaimTypes.Email);
    protected string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
