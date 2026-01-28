namespace Ecommerce.Core;

public class ProductRating : BaseEntity
{
    public ProductRating() { }

    public ProductRating(string userId, int productId, int rating, string? review = null)
    {
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Review = review;
    }

    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    /// <summary>
    /// Rating value from 1 to 5
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional text review from the user
    /// </summary>
    public string? Review { get; set; }
}
