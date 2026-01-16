namespace Ecommerce.Application;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public static string PasswordPattern = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\!@#$%^&*()\[\]{}\-_+=~`|:;""'<>,./?]).{8,}$";

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(PasswordPattern)
            .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display Name is required.")
            .MaximumLength(50).WithMessage("Display Name cannot exceed 50 characters.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User Name is required.")
            .MaximumLength(20).WithMessage("User Name cannot exceed 20 characters.");
    }
}
