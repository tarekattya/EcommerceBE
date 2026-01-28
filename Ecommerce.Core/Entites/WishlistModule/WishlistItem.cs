namespace Ecommerce.Core;

public class WishlistItem : BaseEntity
{
    public WishlistItem() { }

    public WishlistItem(string userId, int productId)
    {
        UserId = userId;
        ProductId = productId;
    }

    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;
}
