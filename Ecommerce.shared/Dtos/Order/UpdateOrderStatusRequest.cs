namespace Ecommerce.Shared;

public record UpdateOrderStatusRequest(string Status, string? TrackingNumber = null);

