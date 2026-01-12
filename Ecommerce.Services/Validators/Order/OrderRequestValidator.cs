using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Application.Validators.Order
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        const int minValue = 0;
        public OrderRequestValidator()
        {
            RuleFor(x => x.buyerEmail).EmailAddress();
            RuleFor(e => e.OrderAddress).SetValidator(new OrderAddressRequestValidator());
            RuleFor(e => e.DeliveryMethodId).GreaterThan(minValue);

        }
    }
}
