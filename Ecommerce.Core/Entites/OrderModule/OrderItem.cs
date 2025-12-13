namespace Ecommerce.Core;

public class OrderItem : BaseEntity
{
    public OrderItem()
    {
    }

    public OrderItem(ProductItemOrderd productItemOrderd, decimal price, int quantity)
    {
        ProductItemOrderd = productItemOrderd;
        Price = price;
        Quantity = quantity;
    }

    public ProductItemOrderd ProductItemOrderd { get; set; }
    public decimal Price { get; set; } = default!;
    public int Quantity { get; set; } = default!;

}

