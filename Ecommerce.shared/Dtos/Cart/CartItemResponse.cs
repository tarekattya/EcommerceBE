namespace Ecommerce.Shared;

public record CartItemResponse(
  int Id,
  int Quantity,
  decimal Price,
  string ProductName,
  string Brand,
  string Type,
  string PictureUrl
    );

