namespace Ecommerce.API;
public class AuthsController(IAuthService authService) :ApiBaseController
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request , CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email , request.Password , cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("Refresh")]
    public async Task<ActionResult<AuthResponse>> GetRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("Revoke")]
    public async Task<ActionResult<AuthResponse>> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result.IsSuccess ? Ok(result.IsSuccess) : result.ToProblem();
    }



}
