using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Helper.Dtos.Product
{
    public record ProductRequest(string Name,
       string Description,
       string PictureUrl,
       decimal Price,
        int BrandId,
        int CategoryId
        );
    
    
}
