namespace Ecommerce.Core;

public class ProductVariant : BaseEntity
{
    public ProductVariant() { }

    public ProductVariant(int productId, string? size, string? color, string sku, decimal price, int stock)
    {
        ProductId = productId;
        Size = size;
        Color = color;
        SKU = sku;
        Price = price;
        Stock = stock;
    }

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string? Size { get; set; }
    public string? Color { get; set; }

    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; private set; }

    public Result ReduceStock(int quantity)
    {
        if (quantity < 0) return Result.Failure(ProductErrors.InValidInputs);
        if (Stock < quantity) return Result.Failure(ProductErrors.TheQuantityNotEnough);
        Stock -= quantity;
        return Result.Success();
    }

    public Result Restock(int quantity)
    {
        if (quantity < 0) return Result.Failure(ProductErrors.InValidInputs);
        Stock += quantity;
        return Result.Success();
    }

    public void UpdateStock(int newStock)
    {
        if (newStock >= 0) Stock = newStock;
    }
}
