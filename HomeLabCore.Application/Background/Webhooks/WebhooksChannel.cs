using HomeLabCore.Application.Constants.Enums;
using HomeLabCore.Application.Logging;
using HomeLabCore.Shared.Contexts;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace HomeLabCore.Application.Background.Webhooks;

public sealed record WebhookMessage(string RawPayload, WebhookSender WebhookSender, string CorrelationId);

public interface IWebhookEventWriter
{
    public void Write(string webhookPayload, WebhookSender webhookSender);
}

public interface IWebhookEventReader
{
    public IAsyncEnumerable<WebhookMessage> ReadAll(CancellationToken ct);
}

internal sealed class WebhooksChannel(ILogger<WebhooksChannel> logger) : IWebhookEventWriter, IWebhookEventReader
{
    private const int QueueCapacity = 10_000;

    private readonly Channel<WebhookMessage> _channel = Channel.CreateBounded<WebhookMessage>(new BoundedChannelOptions(QueueCapacity)
    {
        SingleReader = true,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.Wait
    });

    public void Write(string webhookPayload, WebhookSender webhookSender)
    {
        var message = new WebhookMessage(webhookPayload, webhookSender, CorrelationContext.CorrelationId);

        if (!_channel.Writer.TryWrite(message))
        {
            logger.BackgroundProcessingMessageDropped("Queue is full");
        }

        logger.BackgroundProcessingMessageSent();
    }

    public IAsyncEnumerable<WebhookMessage> ReadAll(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}
