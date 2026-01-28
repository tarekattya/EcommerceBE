namespace Ecommerce.Shared;

public static class WishlistErrors
{
    public static Error ItemNotFound => new Error("WishlistItemNotFound", "Wishlist item not found", statusCode: 404);
    public static Error ItemAlreadyExists => new Error("WishlistItemExists", "This product is already in your wishlist", statusCode: 409);
    public static Error ProductNotFound => new Error("ProductNotFound", "The specified product does not exist", statusCode: 404);
    public static Error InvalidProductId => new Error("InvalidProductId", "Invalid product ID provided", statusCode: 400);
}
