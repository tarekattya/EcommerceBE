namespace Ecommerce.Application;
public class CartRequestValidation : AbstractValidator<CartRequest>
{
    public CartRequestValidation()
    { 
        RuleFor(X => X.Id).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CartItemRequestValidator());
    }
}
