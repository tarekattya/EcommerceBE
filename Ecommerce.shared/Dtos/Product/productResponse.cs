﻿namespace Ecommerce.Shared.Helper.Dtos.Product
{
    public record productResponse(
        int id,
        string Name,
       string Description,
       string PictureUrl,
       decimal Price,
        int BrandId,
        string Brand,
        int CategoryId,
        string Category
        );
}
