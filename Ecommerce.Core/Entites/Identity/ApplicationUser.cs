

namespace Ecommerce.Core.Entites.Identity
{
    public class ApplicationUser :IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public new string UserName { get; set; } = string.Empty;
        public Address? Address { get; set; }
    }
}
