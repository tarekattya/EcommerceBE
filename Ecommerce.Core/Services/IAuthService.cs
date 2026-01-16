

namespace Ecommerce.Core;

public interface IAuthService
{
    public Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    public Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    public Task<Result> ForgotPasswordAsync(string email);
    public Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    public Task<Result> ResendConfirmationEmailAsync(string email);
    public Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);


}
