namespace Ecommerce.Core;
public class Order : BaseEntity
{
    public Order(string buyerEmail, OrderAddress shipingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, bool isCOD = false)
    {
        BuyerEmail = buyerEmail;
        ShipingAddress = shipingAddress;
        DeliveryMethod = deliveryMethod;
        Items = items;
        SubTotal = subTotal;
        IsCOD = isCOD;
    }

    public Order()
    {
    }

    public string BuyerEmail { get; private set; } = string.Empty;
    public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.UtcNow;

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public OrderAddress ShipingAddress { get; private set; } = default!;

    public DeliveryMethod DeliveryMethod { get; private set; } = default!;

    public ICollection<OrderItem> Items { get; private set; } = new HashSet<OrderItem>();

    public decimal SubTotal { get; private set; }

    public bool IsCOD { get; private set; }

    public bool InventoryReserved { get; private set; }

    public Result UpdateStatus(OrderStatus newStatus)
    {
        bool isValid = Status switch
        {
            OrderStatus.Pending => newStatus == OrderStatus.PaymentSucceeded || 
                                   newStatus == OrderStatus.PaymentFailed ||
                                   (IsCOD && newStatus == OrderStatus.Processing),
            OrderStatus.PaymentSucceeded => newStatus == OrderStatus.Processing,
            OrderStatus.Processing => newStatus == OrderStatus.Shipped,
            OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
            _ => false
        };

        if (!isValid) return Result.Failure(OrderErrors.InvalidStatusUpdate);

        Status = newStatus;
        ApplyStatusSideEffects();

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Processing && Status != OrderStatus.PaymentSucceeded)
             return Result.Failure(OrderErrors.CantCancelOrder);

        Status = OrderStatus.Cancelled;
        ApplyStatusSideEffects();
        
        return Result.Success();
    }

    private void ApplyStatusSideEffects()
    {
        switch (Status)
        {
            case OrderStatus.PaymentFailed:
            case OrderStatus.Cancelled:
                if (InventoryReserved)
                {
                    AddDomainEvent(new OrderStockReleaseEvent(Items));
                    InventoryReserved = false;
                }
                break;

            case OrderStatus.PaymentSucceeded when IsCOD:
                Status = OrderStatus.Processing;
                AddDomainEvent(new OrderProcessingStartedEvent(this));
                InventoryReserved = true;
                break;

            case OrderStatus.Processing:
                AddDomainEvent(new OrderProcessingStartedEvent(this));
                InventoryReserved = true;
                break;

            case OrderStatus.Shipped:
                AddDomainEvent(new OrderShippedEvent(this));
                break;

            case OrderStatus.Delivered:
                AddDomainEvent(new OrderDeliveredEvent(this));
                break;
        }
    }

    public decimal GetTotal() =>
        SubTotal + DeliveryMethod.Cost;

}
