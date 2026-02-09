using Ecommerce.Shared;

namespace Ecommerce.Core;
public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request);

    Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string? email = null);

    Task<Result<Pagination<OrderResponse>>> GetOrdersForUserAsync(string email, OrderSpecParams specParams);

    Task<Result<OrderResponse>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);

    Task<Result> CancelOrderAsync(int orderId, string? buyerEmail = null);

    Task<Result<OrderResponse>> GetOrderByTrackingNumberAsync(string trackingNumber);

    /// <summary>Get invoice for an order. Pass null for email when caller is Admin.</summary>
    Task<Result<InvoiceDto>> GetInvoiceAsync(int orderId, string? buyerEmail = null);
}
