using Hangfire;
using Hangfire.Dashboard;
using Ecommerce.Services;

namespace Ecommerce.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder?  builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration , builder);

        WebApplication? app = builder.Build();

        await DbInitializer.InitializeAsync(app);
        
        using (var scope = app.Services.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<Hangfire.IRecurringJobManager>();
            RecurringJobs.RegisterRecurringJobs(recurringJobManager);
        }


        app.UseExceptionMiddleware();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(op =>
            {
                op.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
        app.UseHttpsRedirection();

        if (!app.Environment.IsDevelopment())
        {
            app.UseCors("AllowSpecificOrigin");
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
                DashboardTitle = "Ecommerce Background Jobs"
            });
        }
        
        app.UseStatusCodePagesWithReExecute("/Errors/{0}");
        app.UseStaticFiles();   

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        app.MapControllers();

        app.Run();
    }
}
