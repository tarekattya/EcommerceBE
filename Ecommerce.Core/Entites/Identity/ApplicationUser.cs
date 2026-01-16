

namespace Ecommerce.Core;

    public class ApplicationUser :IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public Address? Address { get; set; }

        public string? OTPCode { get; set; }
        public string? OTPToken { get; set; }
        public DateTime? OTPExpiry { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

