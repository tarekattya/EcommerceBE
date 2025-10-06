using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators
{
    public class CartRequestValidation : AbstractValidator<CartRequest>
    {
        public CartRequestValidation()
        { 
            RuleFor(X => X.Id).NotEmpty();
            RuleForEach(x => x.Items).SetValidator(new CartItemRequestValidator());

        }
    }
}
