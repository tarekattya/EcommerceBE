using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Helper.Dtos.Cart
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
