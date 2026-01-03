
namespace Ecommerce.Shared;

public record CartResponse(string Id, ICollection<CartItemResponse> items);


