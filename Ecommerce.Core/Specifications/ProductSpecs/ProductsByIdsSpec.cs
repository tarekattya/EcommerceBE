namespace Ecommerce.Core;

public class ProductsByIdsSpec : BaseSpecifications<Product>
{
    public ProductsByIdsSpec(IEnumerable<int> ids)
        : base(p => ids.Contains(p.Id))
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
    }
}
