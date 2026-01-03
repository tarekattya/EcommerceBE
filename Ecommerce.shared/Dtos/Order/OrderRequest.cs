namespace Ecommerce.Shared;
public record OrderRequest(string BasketId , OrderAddressRequest OrderAddress , int DeliveryMethodId);


