namespace Ecommerce.Shared;
public record OrderRequest(string buyerEmail, string BasketId, OrderAddressRequest OrderAddress, int DeliveryMethodId, bool IsCOD = false, string? CouponCode = null, int? SavedAddressId = null);


