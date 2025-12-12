
namespace Ecommerce.Shared;

public record CartRequest(string Id, ICollection<CartItemsRequest> Items);



