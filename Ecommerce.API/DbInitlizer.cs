
public static class DbInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();
            await ApplicationDbSeeding.SeedAsync(context);
        }
        catch (Exception ex)
        {
           var loggerFactory = services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("DbInitializer");
logger.LogError(ex, "An error occurred during migration or seeding.");

        }
    }
}
