namespace Ecommerce.Shared;

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Code, string NewPassword);

public record ConfirmEmailRequest(string Email, string Code);

public record ChangePasswordRequest(string OldPassword, string NewPassword);

public record ResendConfirmationRequest(string Email);
