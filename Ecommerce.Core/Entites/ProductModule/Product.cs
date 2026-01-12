namespace Ecommerce.Core;

public class Product : BaseEntity
{
    public Product(string name, string description, string pictureUrl, decimal price, int brandId, int categoryId, int stock)
    {
        Name = name;
        Description = description;
        PictureUrl = pictureUrl;
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
        Stock = stock;
    }

    public Product() { }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string PictureUrl { get; set; } = default!;
    public decimal Price { get; set; } = default!;
    public int BrandId { get; set; }
    public ProductBrand Brand { get; set; } = default!;
    public int CategoryId { get; set; }
    public int Stock { get; private set; }
    public ProductCategory Category { get; set; } = default!;

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
        if (newStock >= 0)
        {
            Stock = newStock;
        }
    }
}
