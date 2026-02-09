using Ecommerce.Core;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Api;
public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public static async Task InitializeAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            await EnsureHangfireDatabaseExistsAsync(services.GetRequiredService<IConfiguration>());

            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            await ApplicationDbSeeding.SeedAsync(context);

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager);
        }
        catch (Exception ex)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DbInitializer");
            logger.LogError(ex, "An error occurred during migration or seeding.");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in new[] { AdminRole, UserRole })
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = DefaultUsers.AdminEmail;
        if (await userManager.FindByEmailAsync(adminEmail) != null) return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Admin",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, DefaultUsers.AdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, AdminRole);
    }

    /// <summary>Creates the Hangfire database if it does not exist (when using a separate DB).</summary>
    private static async Task EnsureHangfireDatabaseExistsAsync(IConfiguration configuration)
    {
        var hangfireConnection = configuration.GetConnectionString("HangfireConnection");
        if (string.IsNullOrWhiteSpace(hangfireConnection)) return;

        try
        {
            var builder = new SqlConnectionStringBuilder(hangfireConnection);
            var databaseName = builder.InitialCatalog;
            if (string.IsNullOrEmpty(databaseName) || string.Equals(databaseName, "master", StringComparison.OrdinalIgnoreCase))
                return;

            builder.InitialCatalog = "master";
            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            var createDbSql = $"""
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'{databaseName.Replace("'", "''")}')
                CREATE DATABASE [{databaseName.Replace("]", "]]")}];
                """;
            await using var cmd = new SqlCommand(createDbSql, connection);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception)
        {
            // Log optional; app may still run if DB already exists or user creates it manually
        }
    }
}
