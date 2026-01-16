namespace Ecommerce.Shared;

public record ProductFiltersResponse(
    IReadOnlyList<FilterItemResponse> Brands,
    IReadOnlyList<FilterItemResponse> Categories,
    decimal MaxPrice
);

public record FilterItemResponse(int Id, string Name, int Count);
