using HomeLabCore.Application.Dto.Media;
using HomeLabCore.Application.Interfaces.Clients;
using HomeLabCore.Application.Interfaces.Database;
using HomeLabCore.Application.Telegram.CommandHandlers.Abstractions;
using HomeLabCore.Application.Telegram.Configuration;
using HomeLabCore.Application.Telegram.Exceptions;
using HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;
using HomeLabCore.Application.Telegram.Services;
using HomeLabCore.Domain.Constants.Enums;
using HomeLabCore.Domain.Entities.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeLabCore.Application.Telegram.CommandHandlers;

internal sealed class SearchCommandHandler(
    IApplicationDbContext dbContext,
    ITelegramBotClient telegramBotClient,
    IMediaManagerClient mediaManagerClient,
    IMessageRenderer messageRenderer,
    IOptionsSnapshot<TelegramSettings> options,
    ILogger<SearchCommandHandler> logger)
    : CommandHandlerBase(telegramBotClient, options, logger)
{
    private const int SearchResultsTotalCount = 20;

    public override CommandHandlerOptions HandlerOptions => new()
    {
        RequiresAuthorization = true,
        CommandName = "search",
        CommandDescription = "Searches the requested movie or series",
        CommandExample = $"/search The Matrix"
    };

    protected override async Task ProcessUpdate(Message message, Message botResponseMessage, CancellationToken ct)
    {
        var searchTerm = GetCommandArgument(message) 
            ?? throw new CommandProcessingException($"Please provide a media name. Example: `{HandlerOptions.CommandExample}`", showToUser: true);
        
        searchTerm = searchTerm.Trim();

        var searchResults = await mediaManagerClient.Search(searchTerm, SearchResultsTotalCount, ct);

        if (searchResults.Count == 0)
        {
            throw new CommandProcessingException($"No results found for \"{searchTerm}\"", showToUser: true);
        }

        var searchSnapshot = new MediaSearchSnapshot
        {
            Query = searchTerm,
            Results = [.. searchResults.Select(m => new MediaSearchSnapshotEntry
            {
                Id = m.Id,
                MediaType = m.MediaType,
                Title = m.Title,
                Overview = m.Overview,
                Status = m.Status,
                ReleaseDate = m.ReleaseDate,
                FirstAirDate = m.FirstAirDate,
                PosterPath = m.PosterPath
            })]
        };

        dbContext.Add(searchSnapshot);
        await dbContext.SaveChanges(ct);

        var mediaToShow = searchResults[0];

        var renderingPayload = await GetMediaRenderingPayload(mediaToShow, ct);

        var searchContext = new MediaSearchContext
        {
            SearchId = searchSnapshot.Id,
            CurrentIndex = 0,
            HasNext = searchResults.Count > 1
        };

        var mediaPage = messageRenderer.RenderMediaSearchPage(renderingPayload, searchContext);

        await RespondWithMessage(mediaPage, ct);
    }

    private async Task<MediaRenderingPayload> GetMediaRenderingPayload(ExternalMediaInfo mediaInfo, CancellationToken ct)
    {
        if (mediaInfo.MediaType is MediaType.Movie)
        {
            return new MovieRenderingPayload
            {
                Id = mediaInfo.Id,
                Title = mediaInfo.Title,
                Overview = mediaInfo.Overview,
                Status = mediaInfo.Status,
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
