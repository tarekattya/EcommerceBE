namespace Ecommerce.Application;

public class CartItemRequestValidator : AbstractValidator<CartItemsRequest>
{
    const int minValue = 0;
    public const int maxValue = 101;
    public CartItemRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(minValue).LessThan(maxValue).WithMessage($"Quantity must be greater than {minValue} and less than {maxValue}");
        RuleFor(x => x.Id).NotNull();
    }
}
