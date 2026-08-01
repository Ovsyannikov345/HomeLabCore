using HomeLabCore.Application.Constants.Enums;
using HomeLabCore.Application.Logging;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Abstractions;

internal abstract class WebhookHandlerBase<TPayload>(ILogger logger) : IWebhookHandler
{
    protected abstract JsonSerializerOptions PayloadSerializerOptions { get; }

    protected readonly ILogger Logger = logger;

    public abstract bool CanHandle(WebhookSender webhookSender);

    public abstract Task ProcessWebhook(TPayload? payload, CancellationToken ct);

    public async Task Handle(string webhookPayload, CancellationToken ct)
    {
        TPayload? deserializedPayload;

        try
        {
            deserializedPayload = JsonSerializer.Deserialize<TPayload>(webhookPayload, PayloadSerializerOptions);
        }
        catch (Exception ex)
        {
            Logger.FailedToDeserializeWebhook(webhookPayload, ex);

            return;
        }

        try
        {
            await ProcessWebhook(deserializedPayload, ct);

            Logger.WebhookProcessed();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.FailedToHandleWebhook(ex);
        }
    }
}
