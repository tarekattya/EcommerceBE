
namespace Ecommerce.Application;

public class OrderService(IUnitOfWork unitOfWork, ICartRepository cartRepository) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }
    public Task<Result<string>> CancelOrderAsync(int orderId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request)
    {
        var cart = await _cartRepository.GetCartAsync(request.BasketId);
        if (cart is null || cart.Items == null || !cart.Items.Any())
            return Result<OrderResponse>.Failure(CartErrors.EmptyCart);

        var deliveryMethod = await _unitOfWork
            .Repository<DeliveryMethod>()
            .GetByIdAsync(request.DeliveryMethodId);

        if (deliveryMethod is null)
            return Result<OrderResponse>.Failure(OrderErrors.InvalidDeliveryMethod);

        foreach (var cartItem in cart.Items)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.Id);

            if (product is null)
                return Result<OrderResponse>.Failure(ProductErrors.NotFoundProduct);

            if (cartItem.Quantity > product.Stock)
                return Result<OrderResponse>.Failure(ProductErrors.TheQuantityNotEnough);
        }
        var items = cart.Items.Select(item => new OrderItem
        {
            ProductItemOrderd = new ProductItemOrderd
            {
                ProductId = item.Id,
                Name = item.ProductName,
                PictureUrl = item.PictureUrl
            },
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();

        var subtotal = items.Sum(item => item.Price * item.Quantity);

        var newOrder = new Order(
            request.buyerEmail,
            request.OrderAddress.Adapt<OrderAddress>(),
            deliveryMethod,
            items,
            subtotal
        );

        await _unitOfWork.Repository<Order>().AddAsync(newOrder);

        foreach (var cartItem in cart.Items)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.Id);
            product.Stock -= cartItem.Quantity;
            _unitOfWork.Repository<Product>().Update(product);
        }
        await _unitOfWork.CompleteAsync();
        var orderResponse = newOrder.Adapt<OrderResponse>();
        return Result<OrderResponse>.Success(orderResponse);
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
