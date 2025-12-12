namespace Ecommerce.Core;

    public class ProductsByNameSpec : BaseSpecifications<Product>
    {
        public ProductsByNameSpec(string Name) : base(p => p.Name == Name)
        {
        }
    }
    
    

