using Ecommerce.shared.Abstraction.Errors.Auth;
using Ecommerce.shared.Dtos.Address;
using System.Security.Claims;

namespace Ecommerce.API.Abstractions
{
    public static class UserManagerExtension
    {
        public static async Task<Result<AddressResponse>> GetUserAddressAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal User, CancellationToken cancellationToken)
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return
               Result<AddressResponse>.Failure(AuthErrors.InvalidCredentials);
            var user = await userManager.Users.Include(u => u.Address).FirstAsync(u => u.Id == userId);
            return Result<AddressResponse>.Success(user.Address.Adapt<AddressResponse>());

        }
        public static async Task<ApplicationUser?> GetUserAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal User, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user;

        }
    }
}
