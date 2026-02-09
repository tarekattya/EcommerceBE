namespace Ecommerce.Application;
public class OrderResponseValidator : AbstractValidator<OrderResponse>
{
    const int minValue = 0;
    public OrderResponseValidator()
    {
        RuleFor(x => x.Id).GreaterThan(minValue);
        RuleFor(x => x.BuyerEmail).EmailAddress();
        RuleFor(x => x.OrderDate).LessThanOrEqualTo(DateTimeOffset.Now);
        RuleForEach(x => x.Items).SetValidator(new OrderItemResponseValidator());
        RuleFor(x => x.DeliveryMethodName).NotEmpty();
        RuleFor(x => x.OrderAddress).SetValidator(new OrderAddressRequestValidator());
        RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(minValue);
        RuleFor(x => x.Total).GreaterThanOrEqualTo(x => x.SubTotal);
    }
}


