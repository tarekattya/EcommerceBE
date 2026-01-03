
namespace Ecommerce.Core;

public interface ICartRepository
{
    public Task<CustomerCart?> GetCartAsync(string key);

    public Task<CustomerCart?> UpdateCartAsync(CustomerCart cart, TimeSpan? time = null);
    public Task<CustomerCart?> CreateCartAsync(CustomerCart cart, TimeSpan? time = null);

    public Task<bool> DeleteCartAsync(string id);


}

