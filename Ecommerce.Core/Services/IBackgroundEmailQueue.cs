namespace Ecommerce.Core;

public interface IBackgroundEmailQueue
{
    ValueTask QueueEmailAsync(EmailMessage emailMessage);
    ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken);
}

public record EmailMessage(
    string To,
    string Subject,
    string Body
);
