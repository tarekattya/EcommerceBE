namespace Ecommerce.Shared.Dtos;

public record DashboardDto(
    int TotalOrders,
    decimal TotalRevenue,
    int PendingOrders,
    int PaymentFailedOrders,
    int ProcessingOrders,
    int CompletedOrders,
    int CancelledOrders,
    int TotalProducts,
    int TotalUsers,
    IReadOnlyList<TopProductDto> TopSellingProducts,
    IReadOnlyList<LowStockProductDto> LowStockProducts,
    IReadOnlyList<RecentOrderDto> RecentOrders
);

public record TopProductDto(int ProductId, string Name, string PictureUrl, int QuantitySold, decimal Revenue);
public record LowStockProductDto(int ProductId, string Name, string PictureUrl, int Stock, string Category);
public record RecentOrderDto(int OrderId, DateTimeOffset OrderDate, string UserEmail, decimal Total, string Status);

