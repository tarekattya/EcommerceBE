namespace Ecommerce.Services;

public class OrderProcessingStartedHandler(IUnitOfWork unitOfWork) : IDomainEventHandler<OrderProcessingStartedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(OrderProcessingStartedEvent domainEvent)
    {
        foreach (OrderItem item in domainEvent.Order.Items)
        {
            // In the new model every order item must have a variant
            if (!item.ProductItemOrderd.ProductVariantId.HasValue)
                continue;

            var variant = await _unitOfWork.Repository<ProductVariant>()
                .GetByIdAsync(item.ProductItemOrderd.ProductVariantId.Value);
            if (variant != null)
            {
                var result = variant.ReduceStock(item.Quantity);
                if (result.IsSuccess)
                    _unitOfWork.Repository<ProductVariant>().Update(variant);
            }
        }
    }
}
