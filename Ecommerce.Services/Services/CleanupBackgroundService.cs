using Ecommerce.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services;

public class CleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupBackgroundService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24); // Run daily

    public CleanupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cleanup Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Cleanup Background Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Cleanup Background Service");
                // Wait before retrying
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Cleanup Background Service stopped");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting cleanup tasks at {Time}", DateTimeOffset.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            // Example: Clean up old soft-deleted records (older than 30 days)
            // This is just an example - implement based on your needs
            var orderRepo = unitOfWork.Repository<Order>();
            
            // You can add cleanup logic here:
            // - Archive old orders
            // - Clean up expired sessions
            // - Remove old temporary files
            // - Clean up old logs
            
            _logger.LogInformation("Cleanup tasks completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cleanup tasks");
            throw;
        }
    }
}
