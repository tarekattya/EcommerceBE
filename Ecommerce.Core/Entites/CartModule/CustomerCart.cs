

namespace Ecommerce.Core;

public class CUstomerCart
{
    public string Id { get; set; } = string.Empty;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
