using Ecommerce.shared.Dtos.Authentcation;

namespace Ecommerce.API.Controllers
{
    public class AuthsController(IAuthService authService) :ApiBaseController
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request , CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.Email , request.Password , cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}
