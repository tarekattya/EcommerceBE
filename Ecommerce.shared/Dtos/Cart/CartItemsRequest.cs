
namespace Ecommerce.Shared;

    public record CartItemsRequest
    (
         int Id,
         int Quantity ,
         decimal Price ,
         string ProductName ,
         string Brand ,
         string Type ,
         string PictureUrl
    );

