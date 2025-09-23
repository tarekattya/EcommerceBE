
namespace Ecommerce.Shared.Helper.Dtos.Cart
{
    public record CartRequest(string Id , ICollection<CartItemsRequest> Items);
    
    
}
