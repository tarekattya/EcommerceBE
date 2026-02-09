using Ecommerce.Core;

namespace Ecommerce.Application.Validators.Order;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeValidOrderStatus)
            .WithMessage("Status must be one of: Pending, PaymentSucceeded, PaymentFailed, Processing, Shipped, Delivered, Cancelled");
    }

    private static bool BeValidOrderStatus(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        (Enum.TryParse<OrderStatus>(value, true, out _) ||
         Enum.TryParse<OrderStatus>(value!.Trim().Replace(" ", ""), true, out _));
}
