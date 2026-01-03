namespace Ecommerce.Core;

public class ProductsByNameAndIdSpec : BaseSpecifications<Product>
{
    public ProductsByNameAndIdSpec(string Name, int id) : base(p => p.Name == Name && p.Id != id)
    {
    }
}



