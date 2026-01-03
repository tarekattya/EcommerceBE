using Ecommerce.Shared;

namespace Ecommerce.Application;

public class OrderAddressRequestValidator : AbstractValidator<OrderAddressRequest>
{
    const int MaxLength = 100;
    public OrderAddressRequestValidator() 
    { 
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(MaxLength).WithMessage($"First name cannot exceed {MaxLength} characters.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(MaxLength).WithMessage($"Last name cannot exceed {MaxLength} characters.");
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(MaxLength).WithMessage($"Street cannot exceed {MaxLength} characters.");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(MaxLength).WithMessage($"City cannot exceed {MaxLength} characters.");
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(MaxLength).WithMessage($"Country cannot exceed {MaxLength} characters.");
    }
}
