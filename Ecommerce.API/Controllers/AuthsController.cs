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



    [HttpPost("Register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
        return result.IsSuccess ? Ok("Email confirmed successfully.") : result.ToProblem();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request.Email);
        return result.IsSuccess ? Ok("If an account exists, a reset link has been sent.") : result.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.IsSuccess ? Ok("Password has been reset successfully.") : result.ToProblem();
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request.Email);
        return result.IsSuccess ? Ok("If an account exists and is not verified, a new link has been sent.") : result.ToProblem();
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (UserId == null) return Unauthorized();
        var result = await _authService.ChangePasswordAsync(UserId, request);
        return result.IsSuccess ? Ok("Password changed successfully.") : result.ToProblem();
    }
}
