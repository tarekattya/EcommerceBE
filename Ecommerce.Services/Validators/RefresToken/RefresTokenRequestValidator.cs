using Ecommerce.shared.Dtos.RefreshToken;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.RefresToken
{
    public class RefresTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefresTokenRequestValidator()
        {
            RuleFor(x=>x.RefreshToken).NotEmpty();
            RuleFor(x=>x.Token).NotEmpty();

        }
    }
}
