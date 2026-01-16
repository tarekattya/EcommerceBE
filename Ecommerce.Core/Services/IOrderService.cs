using Ecommerce.Shared;

namespace Ecommerce.Core;
public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request);

    Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string? email = null);

    Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync(string email);

    Task<Result<OrderResponse>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);

    Task<Result> CancelOrderAsync(int orderId);



}
