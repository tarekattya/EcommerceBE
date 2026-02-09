
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
    public async Task<Result> CancelOrderAsync(int orderId, string? buyerEmail = null)
    {
        OrderSpecification orderSpecification = new OrderSpecification(orderId, buyerEmail);
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
            var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(cartItem.VariantId);
            if (variant is null || variant.ProductId != cartItem.ProductId)
                return Result<OrderResponse>.Failure(ProductErrors.NotFoundProduct);

            if (cartItem.Quantity > variant.Stock)
                return Result<OrderResponse>.Failure(ProductErrors.TheQuantityNotEnough);
        }

        List<OrderItem>? items = cart.Items.Select(item => new OrderItem
        {
            ProductItemOrderd = new ProductItemOrderd
            {
                ProductId = item.ProductId,
                ProductVariantId = item.VariantId,
                Size = item.Size,
                Color = item.Color,
                Name = item.ProductName,
                PictureUrl = item.PictureUrl,
                Description = item.Description ?? string.Empty
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

        newOrder.AddDomainEvent(new OrderCreatedEvent(newOrder));
        await _unitOfWork.Repository<Order>().AddAsync(newOrder);

        await _unitOfWork.CompleteAsync();
        await _cartRepository.DeleteCartAsync(request.BasketId);
        return Result<OrderResponse>.Success(newOrder.Adapt<OrderResponse>());
    }


    public async Task<Result<Pagination<OrderResponse>>> GetOrdersForUserAsync(string email, OrderSpecParams specParams)
    {
        OrdersByEmailSpec orderSpecification = new OrdersByEmailSpec(email, specParams);
        OrdersWithFilterForCountSpec countSpec = new OrdersWithFilterForCountSpec(email, specParams);

        IReadOnlyList<Order> orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpecification);
        int totalItems = await _unitOfWork.Repository<Order>().GetCountAsync(countSpec);

        if (orders == null || !orders.Any())
            return Result<Pagination<OrderResponse>>.Failure(OrderErrors.NotFoundOrder);

        var data = orders.Adapt<IReadOnlyList<OrderResponse>>();

        return Result<Pagination<OrderResponse>>.Success(new Pagination<OrderResponse>(specParams.PageIndex, specParams.PageSize, totalItems, data));
    }

    public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
    { 
        OrderSpecification orderSpecification = new OrderSpecification(orderId);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(orderSpecification);
        
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);

        if (!TryParseOrderStatus(request.Status, out OrderStatus statusEnum))
            return Result<OrderResponse>.Failure(OrderErrors.InvalidStatus);

        Result? result = order.UpdateStatus(statusEnum);
        if (!result.IsSuccess) 
            return Result<OrderResponse>.Failure(OrderErrors.InvalidStatusUpdate);

        if (statusEnum == OrderStatus.Shipped && !string.IsNullOrWhiteSpace(request.TrackingNumber))
            order.SetTrackingNumber(request.TrackingNumber!);

        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CompleteAsync();

        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }

    public async Task<Result<OrderResponse>> GetOrderByTrackingNumberAsync(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        var spec = new OrderByTrackingNumberSpec(trackingNumber);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFoundOrder);
        return Result<OrderResponse>.Success(order.Adapt<OrderResponse>());
    }

    public async Task<Result<InvoiceDto>> GetInvoiceAsync(int orderId, string? buyerEmail = null)
    {
        var spec = new OrderSpecification(orderId, buyerEmail);
        Order? order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
        if (order is null)
            return Result<InvoiceDto>.Failure(OrderErrors.NotFoundOrder);

        var deliveryCost = order.DeliveryMethod?.Cost ?? 0m;
        var invoice = new InvoiceDto(
            InvoiceNumber: $"INV-{order.Id}",
            OrderId: order.Id,
            BuyerEmail: order.BuyerEmail,
            OrderDate: order.OrderDate,
            Status: order.Status.ToString(),
            ShippingAddress: order.ShipingAddress.Adapt<OrderAddressRequest>(),
            DeliveryMethodName: order.DeliveryMethod?.ShortName ?? "",
            Items: order.Items.Adapt<IReadOnlyList<OrderItemResponse>>(),
            SubTotal: order.SubTotal,
            DeliveryCost: deliveryCost,
            Discount: order.Discount,
            Total: order.GetTotal(),
            CouponCode: order.CouponCode,
            TrackingNumber: order.TrackingNumber
        );
        return Result<InvoiceDto>.Success(invoice);
    }

    /// <summary>Parse status from request (accepts enum name or display value e.g. "Payment Succeeded").</summary>
    private static bool TryParseOrderStatus(string? value, out OrderStatus status)
    {
        status = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (Enum.TryParse<OrderStatus>(value, true, out status)) return true;
        var normalized = value.Trim().Replace(" ", "");
        return Enum.TryParse<OrderStatus>(normalized, true, out status);
    }
}
