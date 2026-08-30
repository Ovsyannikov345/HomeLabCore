using HomeLabCore.Application.Constants.Enums;

namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Abstractions;

internal interface IWebhookHandler
{
    public bool CanHandle(WebhookSender webhookSender);

    public Task Handle(string webhookPayload, CancellationToken ct);
}
