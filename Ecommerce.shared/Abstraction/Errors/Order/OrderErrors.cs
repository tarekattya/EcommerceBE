namespace Ecommerce.Shared;
public static class OrderErrors
{
    public  static Error NotFoundOrder => new Error("NotFoundOrder", "Order not found", statusCode: 404);
}
