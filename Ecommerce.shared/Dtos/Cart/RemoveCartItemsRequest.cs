namespace Ecommerce.Shared;

public record RemoveCartItemsRequest(string CartId, List<int> ItemIds);


