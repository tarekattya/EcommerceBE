
namespace Ecommerce.Core;
public interface IDashboardService
{
    Task<Result<DashboardDto>> GetDashboardStatsAsync();
}
