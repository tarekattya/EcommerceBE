namespace Ecommerce.Shared;
public record OrderResponse(int Id ,  string BuyerEmail, DateTimeOffset OrderDate , IReadOnlyList<OrderItemResponse> Items , string DeliveryMethodName ,string OrderStatus , OrderAddressRequest OrderAddress , decimal SubTotal , decimal Total, string? CouponCode = null, decimal Discount = 0);


