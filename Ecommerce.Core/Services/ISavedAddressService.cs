using Ecommerce.Shared;

namespace Ecommerce.Core;

public interface ISavedAddressService
{
    Task<Result<IReadOnlyList<SavedAddressDto>>> GetUserAddressesAsync(string userId);
    Task<Result<SavedAddressDto>> AddAsync(string userId, SavedAddressRequest request);
    Task<Result<SavedAddressDto>> UpdateAsync(int id, string userId, SavedAddressRequest request);
    Task<Result> DeleteAsync(int id, string userId);
    Task<Result> SetDefaultAsync(int id, string userId);
}
