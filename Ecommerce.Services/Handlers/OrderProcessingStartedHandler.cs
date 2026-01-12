namespace Ecommerce.Services;

public class OrderProcessingStartedHandler(IUnitOfWork unitOfWork) : IDomainEventHandler<OrderProcessingStartedEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(OrderProcessingStartedEvent domainEvent)
    {
        foreach (var item in domainEvent.Order.Items)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductItemOrderd.ProductId);
            if (product != null)
            {
                var result = product.ReduceStock(item.Quantity);
                if (result.IsSuccess)
                {
                    _unitOfWork.Repository<Product>().Update(product);
                }
            }
        }
    }
}
