
namespace Ecommerce.Shared;

public record CartItemsRequest(
    int VariantId,
    int ProductId,
    int Quantity
);

