using Ecommerce.Core;
using Ecommerce.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services;

/// <summary>
/// Hangfire recurring job for daily cleanup. Resolved via DI; use IRecurringJobManager to schedule.
/// </summary>
public class DailyCleanupJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyCleanupJob> _logger;

    public DailyCleanupJob(IServiceProvider serviceProvider, ILogger<DailyCleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Daily cleanup job started at {Time}", DateTimeOffset.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var deletedTokens = await db.RefreshTokens
                .Where(r => r.Expires < DateTime.UtcNow)
                .ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {Count} expired refresh token(s)", deletedTokens);

            var otpCleared = await db.Users
                .Where(u => u.OTPExpiry != null && u.OTPExpiry < DateTime.UtcNow)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.OTPCode, (string?)null)
                    .SetProperty(u => u.OTPToken, (string?)null)
                    .SetProperty(u => u.OTPExpiry, (DateTime?)null));
            _logger.LogInformation("Cleared expired OTP for {Count} user(s)", otpCleared);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily cleanup failed");
            throw;
        }

        _logger.LogInformation("Daily cleanup job completed");
    }
}
