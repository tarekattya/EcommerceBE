namespace Ecommerce.Shared;

public record AddCartItemsRequest
(
         string CartId,
         List<CartItemsRequest> Items
);
