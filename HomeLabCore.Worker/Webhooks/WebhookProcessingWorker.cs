using HomeLabCore.Application.Background.Webhooks;
using HomeLabCore.Application.Webhooks;
using HomeLabCore.Shared.Constants;
using HomeLabCore.Shared.Contexts;
using HomeLabCore.Worker.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace HomeLabCore.Worker.Webhooks;

internal sealed class WebhookProcessingWorker(
    IServiceScopeFactory scopeFactory,
    IWebhookEventReader webhookEventReader,
    ILogger<WebhookProcessingWorker> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.StartingWebhookProcessing();

        try
        {
            await foreach (var webhookEvent in webhookEventReader.ReadAll(stoppingToken))
            {
                CorrelationContext.CorrelationId = webhookEvent.CorrelationId;
                using var correlationContext = LogContext.PushProperty(LogPropertyNames.CorrelationId, CorrelationContext.CorrelationId);

                try
                {
                    await ProcessWebhookEvent(webhookEvent, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.WebhookProcessingError(ex);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.WebhookProcessingStopped();
        }
    }

    private async Task ProcessWebhookEvent(WebhookMessage webhookEvent, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

        await webhookService.ProcessWebhook(webhookEvent.RawPayload, webhookEvent.WebhookSender, ct);
    }
}
