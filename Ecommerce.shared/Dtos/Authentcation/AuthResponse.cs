 namespace Ecommerce.Shared;
public record AuthResponse(string Id, string? Email, string DisplayName, string UserName, string Token, int ExpiersIn, string RefreshToken, DateTime RefreshTokenExpiersIn);



