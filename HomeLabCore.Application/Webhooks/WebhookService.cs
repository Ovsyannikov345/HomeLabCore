using HomeLabCore.Application.Constants.Enums;
using HomeLabCore.Application.Logging;
using HomeLabCore.Application.Webhooks.WebhookHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace HomeLabCore.Application.Webhooks;

public interface IWebhookService
{
    public Task ProcessWebhook(string rawPayload, WebhookSender webhookSender, CancellationToken ct);
}

internal sealed class WebhookService(
    IEnumerable<IWebhookHandler> webhookHandlers, 
    ILogger<WebhookService> logger) 
    : IWebhookService
{
    public async Task ProcessWebhook(string rawPayload, WebhookSender webhookSender, CancellationToken ct)
    {
        var handler = webhookHandlers.FirstOrDefault(h => h.CanHandle(webhookSender));

        if (handler is null)
        {
            logger.FailedToHandleWebhook("Failed to determine webhook handler");

            return;
        }

        await handler.Handle(rawPayload, ct);
    }
}
