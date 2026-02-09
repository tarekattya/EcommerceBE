namespace Ecommerce.API.Controllers;

[Authorize(Roles = "Admin")]
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

    [HttpGet("sales-report")]
    public async Task<IActionResult> GetSalesReport([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
    {
        var end = to ?? DateTimeOffset.UtcNow;
        var start = from ?? end.AddDays(-30);
        var result = await _dashboardService.GetSalesReportAsync(start, end);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>Orders report (admin): filter by date range, status, amount. Paginated. Use format=csv to download CSV.</summary>
    [HttpGet("reports/orders")]
    public async Task<IActionResult> GetOrdersReport([FromQuery] OrderSpecParams specParams, [FromQuery] string? format = null)
    {
        var result = await _dashboardService.GetOrdersReportAsync(specParams);
        if (!result.IsSuccess) return result.ToProblem();

        if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = BuildOrdersReportCsv(result.Value!.Data);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"orders-report-{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        return Ok(result.Value);
    }

    private static string BuildOrdersReportCsv(IReadOnlyList<OrderResponse> orders)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,BuyerEmail,OrderDate,Status,SubTotal,Total,CouponCode,Discount,TrackingNumber");
        foreach (var o in orders)
        {
            sb.AppendLine($"{o.Id},\"{o.BuyerEmail}\",{o.OrderDate:O},{o.OrderStatus},{o.SubTotal},{o.Total},\"{o.CouponCode ?? ""}\",{o.Discount},\"{o.TrackingNumber ?? ""}\"");
        }
        return sb.ToString();
    }
}
