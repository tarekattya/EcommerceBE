
namespace Ecommerce.Application;

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

    public string? ValidateToken(string token)
    {
        var tokenhandler = new JwtSecurityTokenHandler();
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        try
        {
            tokenhandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ClockSkew = TimeSpan.Zero

            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
           return jwtToken.Claims.First(x=> x.Type == JwtRegisteredClaimNames.Sub).Value.ToString();
        }
        catch 
        {

            return null;
        }
    }

   
}
