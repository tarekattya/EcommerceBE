namespace Ecommerce.Services;

public class OrderStockReleaseHandler(IUnitOfWork unitOfWork) : IDomainEventHandler<OrderStockReleaseEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(OrderStockReleaseEvent domainEvent)
    {
        foreach (var item in domainEvent.Items)
        {
            if (!item.ProductItemOrderd.ProductVariantId.HasValue)
                continue;

            var variant = await _unitOfWork.Repository<ProductVariant>()
                .GetByIdAsync(item.ProductItemOrderd.ProductVariantId.Value);
            if (variant != null)
            {
                var result = variant.Restock(item.Quantity);
                if (result.IsSuccess)
                    _unitOfWork.Repository<ProductVariant>().Update(variant);
            }
        }
    }
}
