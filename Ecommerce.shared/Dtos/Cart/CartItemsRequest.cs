
namespace Ecommerce.Shared.Helper.Dtos.Cart
{
    public record CartItemsRequest
    (
         int Id,
         int Quantity ,
         decimal Price ,
         string ProductName ,
         string PictureUrl
    );
}
