namespace Ecommerce.API.Controllers;

public class DashboardController(IDashboardService dashboardService) : ApiBaseController
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("stats")]
    [Cached(30)] // Cache dashboard stats for 30 seconds to improve performance
    public async Task<IActionResult> GetDashboardStats()
    {
        Result<DashboardDto>? result = await _dashboardService.GetDashboardStatsAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
