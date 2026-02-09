using Ecommerce.Shared;
using Ecommerce.Shared.Dtos;

namespace Ecommerce.Core;
public interface IDashboardService
{
    Task<Result<DashboardDto>> GetDashboardStatsAsync();
    Task<Result<SalesReportDto>> GetSalesReportAsync(DateTimeOffset from, DateTimeOffset to);
    Task<Result<Pagination<OrderResponse>>> GetOrdersReportAsync(OrderSpecParams specParams);
}
