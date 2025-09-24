using Ecommerce.Application.Options;
using Ecommerce.Core.Entites.Identity;
using Ecommerce.Infrastructure.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Providers
{
    public class JwtProvider(IOptions<JwtSettings> options) : IJwtProvider
    {
        private readonly JwtSettings _options = options.Value;

        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [

                new Claim(JwtRegisteredClaimNames.Sub , user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email!),
                new Claim(JwtRegisteredClaimNames.Name , user.DisplayName),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
                ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey , SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(

                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes)
                );


            return (token: new JwtSecurityTokenHandler().WriteToken(token), _options.ExpireMinutes);

                
                
        }
    }
}
