namespace Ecommerce.Shared;
public static class OrderErrors
{
    public  static Error NotFoundOrder => new Error("NotFoundOrder", "Order not found", statusCode: 404);
    public  static Error InvalidDeliveryMethod => new Error("InvalidDeliveryMethod", "Invalid Delivery Method", statusCode: 404);
    public  static Error CantCancelOrder => new Error("CantCancelOrder", "Can't cancel order", statusCode: 400);



}
