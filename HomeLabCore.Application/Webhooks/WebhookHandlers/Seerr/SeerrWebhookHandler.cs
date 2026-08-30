using HomeLabCore.Application.Constants.Enums;
using HomeLabCore.Application.Interfaces.Database;
using HomeLabCore.Application.Logging;
using HomeLabCore.Application.Webhooks.WebhookHandlers.Abstractions;
using HomeLabCore.Application.Webhooks.WebhookHandlers.Exceptions;
using HomeLabCore.Domain.Constants.Enums;
using HomeLabCore.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot;

namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Seerr;

internal sealed class SeerrWebhookHandler(
    IApplicationDbContext dbContext,
    ITelegramBotClient telegramBotClient,
    ILogger<SeerrWebhookHandler> logger)
    : WebhookHandlerBase<SeerrWebhookPayload>(logger)
{
    protected override JsonSerializerOptions PayloadSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public override bool CanHandle(WebhookSender webhookSender) => webhookSender is WebhookSender.Seerr;

    public override async Task ProcessWebhook(SeerrWebhookPayload? payload, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(payload);

        await (payload.NotificationType switch
        {
            SeerNotificationTypes.TestNotification => HandleTestNotification(),

            SeerNotificationTypes.MediaPending
                or SeerNotificationTypes.MediaApproved
                or SeerNotificationTypes.MediaAvailable
                or SeerNotificationTypes.MediaFailed
                or SeerNotificationTypes.MediaDeclined
                or SeerNotificationTypes.MediaAutoRequested
                or SeerNotificationTypes.MediaAutoApproved
                    => HandleMediaNotification(payload, ct),


            _ => HandleUnknownNotification(payload, ct)
        });
    }

    private Task HandleTestNotification()
    {
        Logger.SeerrTestNotificationReceived();

        return Task.CompletedTask;
    }

    private async Task HandleMediaNotification(SeerrWebhookPayload payload, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(payload.Media);

        var mediaType = payload.Media.MediaType switch
        {
            "movie" => MediaType.Movie,
            "tv" => MediaType.Series,
            _ => throw new WebhookProcessingException($"Unknown media type: {payload.Media.MediaType}")
        };

        if (!int.TryParse(payload.Media.TmdbId, out var mediaId))
        {
            throw new WebhookProcessingException($"Failed to parse media id. Raw value: '{payload.Media.TmdbId}'");
        }

        var mediaSubscriptions = await dbContext
            .Query<MediaSubscription>()
            .Where(s => s.MediaType == mediaType && s.MediaExternalId == mediaId)
            .ToListAsync(ct);

        foreach (var subscription in mediaSubscriptions.DistinctBy(s => s.UserId))
        {
            // TODO use renderer
            await telegramBotClient.SendMessage(subscription.ChatId, payload.Event, cancellationToken: ct);
        }

        if (payload.NotificationType is SeerNotificationTypes.MediaAvailable
                                     or SeerNotificationTypes.MediaFailed
                                     or SeerNotificationTypes.MediaDeclined)
        {
            dbContext.RemoveRange(mediaSubscriptions);
            await dbContext.SaveChanges(ct);
            
            Logger.DeletedMediaSubscriptions(mediaSubscriptions.Count, mediaType, mediaId);
        }
    }

    private Task HandleUnknownNotification(SeerrWebhookPayload payload, CancellationToken ct)
    {
        Logger.SeerrUnknownNotificationReceived(payload.NotificationType);
        
        return Task.CompletedTask;
    }
}
