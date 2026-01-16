
namespace Ecommerce.Application;

public class OrderService(IUnitOfWork unitOfWork, ICartRepository cartRepository, ICouponService couponService) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly ICouponService _couponService = couponService;

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string? email = null)
    {
        OrderSpecification orderSpecification = new OrderSpecification(id, email);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecification);
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }
    public async Task<Result> CancelOrderAsync(int orderId)
    {
        OrderSpecification orderSpecification = new OrderSpecification(orderId);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecification);
        
        if (order is null)
            return Result.Failure(OrderErrors.NotFoundOrder);

        Result? result = order.Cancel();
        if (!result.IsSuccess) return result;

        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(OrderRequest request)
    {   
        CustomerCart? cart = await _cartRepository.GetCartAsync(request.BasketId);
        if (cart is null || cart.Items == null || !cart.Items.Any())
            return Result<OrderResponse>.Failure(CartErrors.EmptyCart);

        DeliveryMethod? deliveryMethod = await _unitOfWork
            .Repository<DeliveryMethod>()
            .GetByIdAsync(request.DeliveryMethodId);

        if (deliveryMethod is null)
            return Result<OrderResponse>.Failure(OrderErrors.InvalidDeliveryMethod);

        foreach (CartItem cartItem in cart.Items)
        {
            Product? product = await _unitOfWork.Repository<Product>().GetByIdAsync(cartItem.Id);

            if (product is null)
                return Result<OrderResponse>.Failure(ProductErrors.NotFoundProduct);

            if (cartItem.Quantity > product.Stock)
                return Result<OrderResponse>.Failure(ProductErrors.TheQuantityNotEnough);
        }
        List<OrderItem>? items = cart.Items.Select(item => new OrderItem
        {
            ProductItemOrderd = new ProductItemOrderd
            {
                ProductId = item.Id,
                Name = item.ProductName,
                PictureUrl = item.PictureUrl,
                Description =item.Description

            },
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();

        decimal subtotal = items.Sum(item => item.Price * item.Quantity);
        decimal discount = 0;

        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            var couponResult = await _couponService.ValidateAndApplyCouponAsync(request.CouponCode, subtotal, deliveryMethod.Cost, items);
            if (couponResult.IsSuccess)
            {
                discount = couponResult.Value;
                
                // Increment Usage Count
                var coupon = await _unitOfWork.Repository<Coupon>().GetByIdWithSpecAsync(new CouponByCodeSpec(request.CouponCode));
                if (coupon != null)
                {
                    coupon.UsageCount++;
                    _unitOfWork.Repository<Coupon>().Update(coupon);
                }
            }
            else
            {
                return Result<OrderResponse>.Failure(couponResult.Error!);
            }
        }

        Order? newOrder = new Order(
            request.buyerEmail,
            request.OrderAddress.Adapt<OrderAddress>(),
            deliveryMethod,
            items,
            subtotal,
            request.IsCOD,
            request.CouponCode,
            discount
        );

        await _unitOfWork.Repository<Order>().AddAsync(newOrder);

        await _unitOfWork.CompleteAsync();
        await _cartRepository.DeleteCartAsync(request.BasketId);
        return Result<OrderResponse>.Success(newOrder.Adapt<OrderResponse>());
    }


    public async Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync(string email)
    {
        OrdersByEmailSpec orderSpecification = new OrdersByEmailSpec(email);
        IReadOnlyList<Order> orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpecification);
        
        if (orders == null || !orders.Any())
            return Result<IReadOnlyList<OrderResponse>>.Failure(OrderErrors.NotFoundOrder);
        
        return Result<IReadOnlyList<OrderResponse>>.Success(orders.Adapt<IReadOnlyList<OrderResponse>>());
    }

    public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
    { 
        OrderSpecification orderSpecification = new OrderSpecification(orderId);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecification);
        
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out OrderStatus statusEnum))
        {
            return Result<OrderResponse>.Failure(OrderErrors.InvalidStatus);
        }

        Result? result = order.UpdateStatus(statusEnum);
        if (!result.IsSuccess) 
            return Result<OrderResponse>.Failure(OrderErrors.InvalidStatusUpdate);

        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CompleteAsync();

        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }


}
