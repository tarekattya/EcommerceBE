using Ecommerce.Shared;

namespace Ecommerce.Core;
public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request);

    Task<Result<OrderResponse>> GetOrderByIdAsync(int id);

    Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersByUserIdAsync(Guid userId);

    Task<Result<string>> UpdateOrderStatusAsync(int orderId, OrderStatus status);

    Task<Result<string>> CancelOrderAsync(int orderId);

}
