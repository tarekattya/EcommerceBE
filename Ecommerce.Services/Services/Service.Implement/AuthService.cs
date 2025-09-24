using Ecommerce.Core.Entites.Identity;
using Ecommerce.Core.Services.Service.Contarct;
using Ecommerce.Infrastructure.Providers;
using Ecommerce.shared.Abstraction.Errors.Auth;
using Ecommerce.shared.Dtos.Authentcation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services.Service.Implement
{
    public class AuthService(UserManager<ApplicationUser> userManager , IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);
          var IsValidPassword = await _userManager.CheckPasswordAsync(user, Password);
            if(!IsValidPassword)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);


            var (token, expiresIn) =  _jwtProvider.GenerateToken(user);

            var response = new AuthResponse(user.Id, user.Email, user.DisplayName, user.UserName , token, expiresIn);
            return Result<AuthResponse>.Success(response);








        }
    }
}
