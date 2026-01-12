using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Validators.Order
{
    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .Must(s => Enum.TryParse<OrderStatus>(s.Replace(" ", ""), true, out _))
                .WithMessage("Invalid order status");
        }
    }
}
