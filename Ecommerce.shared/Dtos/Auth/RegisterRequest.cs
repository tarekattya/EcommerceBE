namespace Ecommerce.Shared;

public record RegisterRequest(string Email, string Password, string DisplayName, string UserName);
