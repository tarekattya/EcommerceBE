namespace Ecommerce.API.Controllers;

public class DashboardController(IDashboardService dashboardService) : ApiBaseController
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _dashboardService.GetDashboardStatsAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
