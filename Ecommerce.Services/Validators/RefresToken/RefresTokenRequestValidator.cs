namespace Ecommerce.Application;

public class RefresTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefresTokenRequestValidator()
    {
        RuleFor(x=>x.RefreshToken).NotEmpty();
        RuleFor(x=>x.Token).NotEmpty();

    }
}
