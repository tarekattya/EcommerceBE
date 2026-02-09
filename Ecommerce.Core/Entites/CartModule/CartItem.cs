

namespace Ecommerce.Core;

public class CartItem
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }

    public string? Size { get; set; }
    public string? Color { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
    public string Description { get; set; } = default!;
}

