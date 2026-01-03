
namespace Ecommerce.Application;

public class OrderService(IGenericRepository<Order> repository) : IOrderService
{
    private readonly IGenericRepository<Order> _repository = repository;

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null)
             return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }
    public Task<Result<string>> CancelOrderAsync(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        throw new NotImplementedException();
    }
}
