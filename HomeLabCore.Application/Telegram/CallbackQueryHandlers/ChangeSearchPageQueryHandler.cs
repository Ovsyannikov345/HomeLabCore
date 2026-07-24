using HomeLabCore.Application.Dto.Media;
using HomeLabCore.Application.Interfaces.Clients;
using HomeLabCore.Application.Interfaces.Database;
using HomeLabCore.Application.Logging;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Abstractions;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;
using HomeLabCore.Application.Telegram.Configuration;
using HomeLabCore.Application.Telegram.Constants;
using HomeLabCore.Application.Telegram.Exceptions;
using HomeLabCore.Application.Telegram.MessageRendering;
using HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;
using HomeLabCore.Domain.Constants.Enums;
using HomeLabCore.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace HomeLabCore.Application.Telegram.CallbackQueryHandlers;

internal sealed class ChangeSearchPageQueryHandler(
    IApplicationDbContext dbContext,
    ITelegramBotClient telegramBotClient,
    IMediaManagerClient mediaManagerClient,
    IMessageRenderer messageRenderer,
    IOptionsSnapshot<TelegramSettings> options,
    ILogger<ChangeSearchPageQueryHandler> logger)
    : CallbackQueryHandlerBase<ChangeSearchPagePayload>(telegramBotClient, options, logger)
{
    protected override string QueryPrefix => CallbackQueryConstants.Prefixes.ChangePage;

    protected override bool RequiresAuthorization => true;

    protected override async Task ProcessCallbackQuery(CallbackQueryContext context, ChangeSearchPagePayload payload, CancellationToken ct)
    {
        var searchSnapshot = await dbContext
            .Query<MediaSearchSnapshot>()
            .FirstOrDefaultAsync(s => s.Id == payload.SearchId, ct);

        if (searchSnapshot is null || searchSnapshot.Results.Count == 0)
        {
            throw new CallbackQueryProcessingException("This search session has expired. Please search again.", showToUser: true);
        }

        if (payload.NextIndex < 0 || payload.NextIndex >= searchSnapshot.Results.Count)
        {
            throw new CallbackQueryProcessingException("Invalid index in payload.", showToUser: false);
        }

        var snapshotEntry = searchSnapshot.Results[payload.NextIndex];

        var mediaInfo = ExternalMediaInfo.FromSnapshot(snapshotEntry);

        var renderingPayload = await GetMediaRenderingPayload(mediaInfo, ct);

        var hasNext = payload.NextIndex < searchSnapshot.Results.Count - 1;

        var searchContext = new MediaSearchContext
        {
            SearchId = payload.SearchId,
            CurrentIndex = payload.NextIndex,
            HasNext = hasNext
        };

        var mediaPage = messageRenderer.RenderMediaSearchPage(renderingPayload, searchContext);

        await RespondWithMessage(mediaPage, ct);
        await DeleteOriginalMessage(CancellationToken.None);
    }

    private async Task<MediaRenderingPayload> GetMediaRenderingPayload(ExternalMediaInfo mediaInfo, CancellationToken ct)
    {
        if (mediaInfo.MediaType is MediaType.Movie)
        {
            var latestStatus = mediaInfo.Status;

            try
            {
                latestStatus = await mediaManagerClient.GetMediaStatus(mediaInfo.MediaType, mediaInfo.Id, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.FailedToFetchLatestMediaStatus(mediaInfo.MediaType, mediaInfo.Id);
            }

            return new MovieRenderingPayload
            {
                Id = mediaInfo.Id,
                Title = mediaInfo.Title,
                Overview = mediaInfo.Overview,
                Status = latestStatus,
                ReleaseDate = mediaInfo.ReleaseDate,
                FirstAirDate = mediaInfo.FirstAirDate,
                PosterPath = mediaInfo.PosterPath
            };
        }

        if (mediaInfo.MediaType is MediaType.Series)
        {
            var seriesDetails = await mediaManagerClient.GetSeriesDetails(mediaInfo.Id, ct);

            return new SeriesRenderingPayload
            {
                Id = seriesDetails.Id,
                Title = seriesDetails.Name,
                Overview = seriesDetails.Overview,
                FirstAirDate = seriesDetails.FirstAirDate,
                PosterPath = mediaInfo.PosterPath,
                Seasons = [.. seriesDetails.Seasons.Select(s => new SeriesRenderingPayload.Season
                {
                    Id = s.Id,
                    Number = s.SeasonNumber,
                    Status = s.Status
                })]
            };
        }

        throw new ArgumentOutOfRangeException(nameof(mediaInfo), "Unknown media type");
    }
}
