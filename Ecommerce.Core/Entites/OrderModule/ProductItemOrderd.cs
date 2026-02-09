
namespace Ecommerce.Core;

public class ProductItemOrderd
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string PictureUrl { get; set; } = default!;
}
