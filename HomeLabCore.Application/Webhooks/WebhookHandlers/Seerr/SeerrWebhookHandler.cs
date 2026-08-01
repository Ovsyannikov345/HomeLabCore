using HomeLabCore.Application.Constants.Enums;
using HomeLabCore.Application.Webhooks.WebhookHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Seerr;

internal sealed class SeerrWebhookHandler(ILogger<SeerrWebhookHandler> logger) 
    : WebhookHandlerBase<SeerrWebhookPayload>(logger)
{
    protected override JsonSerializerOptions PayloadSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public override bool CanHandle(WebhookSender webhookSender) => webhookSender is WebhookSender.Seerr;

    public override Task ProcessWebhook(SeerrWebhookPayload? payload, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
