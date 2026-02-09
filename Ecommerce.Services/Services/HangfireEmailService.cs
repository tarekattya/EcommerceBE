using Ecommerce.Core;
using Hangfire;

namespace Ecommerce.Services;

public class HangfireEmailService : IEmailService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireEmailService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public Task SendEmailAsync(string mailTo, string subject, string body)
    {
        _backgroundJobClient.Enqueue<EmailService>(service =>
            service.SendEmailAsync(mailTo, subject, body));
        return Task.CompletedTask;
    }
}
