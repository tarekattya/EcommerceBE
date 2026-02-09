using Ecommerce.Core;

namespace Ecommerce.Services;

public class QueuedEmailService(IBackgroundEmailQueue emailQueue) : IEmailService
{
    private readonly IBackgroundEmailQueue _emailQueue = emailQueue;

    public async Task SendEmailAsync(string mailTo, string subject, string body)
    {
        var emailMessage = new EmailMessage(mailTo, subject, body);
        await _emailQueue.QueueEmailAsync(emailMessage);
    }
}
