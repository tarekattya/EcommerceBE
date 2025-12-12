namespace Ecommerce.Core;

public class Product : AuditLogging
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string PictureUrl { get; set; } = default!;
    public decimal Price { get; set; } = default!;



    public int BrandId { get; set; }
    public ProductBrand Brand { get; set; } = default!;
    public int CategoryId { get; set; }
    public ProductCategory Category { get; set; } = default!;

}
