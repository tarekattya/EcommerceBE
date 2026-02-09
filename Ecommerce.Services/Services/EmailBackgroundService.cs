using Ecommerce.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services;

public class EmailBackgroundService : BackgroundService
{
    private readonly IBackgroundEmailQueue _emailQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(
        IBackgroundEmailQueue emailQueue,
        IServiceProvider serviceProvider,
        ILogger<EmailBackgroundService> logger)
    {
        _emailQueue = emailQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var emailMessage = await _emailQueue.DequeueAsync(stoppingToken);
                
                _logger.LogInformation("Processing email to {Recipient}", emailMessage.To);

                try
                {
                    // Use direct EmailService (not queued) to avoid circular dependency
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                    
                    await emailService.SendEmailAsync(
                        emailMessage.To,
                        emailMessage.Subject,
                        emailMessage.Body);

                    _logger.LogInformation("Email sent successfully to {Recipient}", emailMessage.To);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to {Recipient}. Subject: {Subject}", 
                        emailMessage.To, emailMessage.Subject);
                    
                    // Optionally: Retry logic or dead letter queue could be added here
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Email Background Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Email Background Service");
                // Wait a bit before retrying to avoid tight loop on persistent errors
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Email Background Service stopped");
    }
}
