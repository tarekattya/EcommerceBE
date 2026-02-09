namespace Ecommerce.Shared;

/// <summary>Identifies a cart line by variant.</summary>
public record RemoveCartItemsRequest(string CartId, List<int> VariantIds);


