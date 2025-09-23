using Ecommerce.Shared.Helper.Dtos.Cart;
using FluentValidation;


namespace Ecommerce.Application.Validators
{
    public class CartItemRequestValidator : AbstractValidator<CartItemsRequest>
    {
        public CartItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).LessThan(101).WithMessage("Quantity must be greater than 0 and less than 101.");
            RuleFor(x=> x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            

        }
    }
}
