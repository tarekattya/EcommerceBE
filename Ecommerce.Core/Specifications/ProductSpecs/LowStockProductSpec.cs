
namespace Ecommerce.Core;

public class LowStockProductSpec : BaseSpecifications<Product>
{
    public LowStockProductSpec(int threshold) : base(p => p.Stock <= threshold && !p.IsDeleted)
    {
        Includes.Add(p => p.Category);
        AddOrderBy(p => p.Stock);
        ApplyPagination(0, 5); 
    }
}
