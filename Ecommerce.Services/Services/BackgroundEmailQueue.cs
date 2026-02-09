using System.Threading.Channels;
using Ecommerce.Core;

namespace Ecommerce.Services;

public class BackgroundEmailQueue : IBackgroundEmailQueue
{
    private readonly Channel<EmailMessage> _queue;

    public BackgroundEmailQueue()
    {
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<EmailMessage>(options);
    }

    public async ValueTask QueueEmailAsync(EmailMessage emailMessage)
    {
        await _queue.Writer.WriteAsync(emailMessage);
    }

    public async ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
