namespace Ecommerce.Shared.Dtos;

public record SalesReportDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int OrderCount,
    decimal Revenue,
    int CancelledCount
);
