using Hangfire.Dashboard;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.API;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In development, allow all access
        // In production, add proper authorization here
        var httpContext = context.GetHttpContext();
        return httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
    }
}
