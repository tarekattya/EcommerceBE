using Ecommerce.Shared;

namespace Ecommerce.Application;

public class OrderItemResponseValidator: AbstractValidator<OrderItemResponse>
{
    const int minValue = 0;
    public OrderItemResponseValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(minValue);
        RuleFor(x => x.Price).GreaterThan(minValue);
        RuleFor(x => x.PictureUrl).NotEmpty();
    }

}
