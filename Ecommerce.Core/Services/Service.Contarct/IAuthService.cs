using Ecommerce.shared.Dtos.Authentcation;
using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Services.Service.Contarct
{
    public interface IAuthService
    {
        public Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
    }
}
