using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.shared.Dtos.Authentcation
{
    public record AuthResponse(string Id , string? Email , string DisplayName , string UserName , string Token , int ExpiersIn , string RefreshToken , DateTime RefreshTokenExpiersIn);
    
    
}
