namespace Ecommerce.Services;

public class OrderStockReleaseHandler(IUnitOfWork unitOfWork) : IDomainEventHandler<OrderStockReleaseEvent>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(OrderStockReleaseEvent domainEvent)
    {
        foreach (var item in domainEvent.Items)
        {
            Product? product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductItemOrderd.ProductId);
            if (product != null)
            {
                Result? result = product.Restock(item.Quantity);
                if (result.IsSuccess)
                {
                    _unitOfWork.Repository<Product>().Update(product);
                }
            }
        }
    }
}
