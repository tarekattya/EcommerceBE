namespace Ecommerce.Shared;

/// <summary>Invoice view for an order (for printing or PDF).</summary>
public record InvoiceDto(
    string InvoiceNumber,
    int OrderId,
    string BuyerEmail,
    DateTimeOffset OrderDate,
    string Status,
    OrderAddressRequest ShippingAddress,
    string DeliveryMethodName,
    IReadOnlyList<OrderItemResponse> Items,
    decimal SubTotal,
    decimal DeliveryCost,
    decimal Discount,
    decimal Total,
    string? CouponCode,
    string? TrackingNumber
);
