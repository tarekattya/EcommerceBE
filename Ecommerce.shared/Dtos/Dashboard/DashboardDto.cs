namespace Ecommerce.Shared.Dtos;

public record DashboardDto(
    int TotalOrders,
    decimal TotalRevenue,
    int PendingOrders,
    int PaymentFailedOrders,
    int CompletedOrders,
    int CancelledOrders,
    int TotalProducts,
    int TotalUsers
);
