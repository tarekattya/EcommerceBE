namespace Ecommerce.Shared;
public record OrderResponse(int Id ,  string BuyerEmail, DateTimeOffset OrderDate 
    , IReadOnlyList<OrderItemResponse> Items , string DeliveryMethodName , int OrderStatus , OrderAddressRequest OrderAddress , decimal SubTotal , decimal Total);


