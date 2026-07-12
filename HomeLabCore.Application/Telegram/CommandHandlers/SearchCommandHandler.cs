using HomeLabCore.Application.Interfaces.Clients;
using HomeLabCore.Application.Interfaces.Database;
using HomeLabCore.Application.Telegram.Services;
using HomeLabCore.Domain.Entities.Media;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeLabCore.Application.Telegram.CommandHandlers;

public sealed class SearchCommandHandler(
    IApplicationDbContext dbContext,
    ITelegramBotClient telegramBotClient,
    IMediaManagerClient mediaManagerClient,
    IMessageRenderer messageRenderer,
    ILogger<SearchCommandHandler> logger)
    : CommandHandlerBase(telegramBotClient), ICommandHandler
{
    private const int SearchResultsTotalCount = 20;

    public override string CommandName => "search";

    public override string CommandDescription => "Searches the requested movie or series";

    public override string? CommandExample => $"/{CommandName} The Matrix";

    protected override async Task ProcessUpdate(Message message, CancellationToken ct)
    {
        // TODO handle already downloaded
        // TODO handle season selection + already downloaded seasons

        // TODO do all handling lifecycle in base class
        var loadingMessage = await BotClient.SendMessage(
            chatId: message.Chat.Id,
            text: "🔍 Searching media...",
            cancellationToken: ct);

        await Task.Delay(1000, ct);

        var searchTerm = GetCommandArgument(message);

        if (searchTerm is null)
        {
            // TODO throw exception and handle in base class
            await BotClient.EditMessageText(
                    chatId: message.Chat.Id,
                    messageId: loadingMessage.MessageId,
                    text: $"❌ Please provide a movie name. Example: `{CommandExample}`",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);

            return;
        }

        await Task.Delay(2000, ct);

        searchTerm = searchTerm.Trim();

        // TODO proper logging
        logger.LogInformation("Chat {ChatId} searching for: {Query}", message.Chat.Id, searchTerm);

        var searchResults = await mediaManagerClient.Search(searchTerm, SearchResultsTotalCount, ct);

        if (searchResults.Count == 0)
        {
            // TODO throw exception and handle in base class
            await BotClient.EditMessageText(
                    chatId: message.Chat.Id,
                    messageId: loadingMessage.MessageId,
                    text: $"❌ No results found for \"{searchTerm}\".",
                    cancellationToken: ct);

            return;
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
                ReleaseDate = m.ReleaseDate,
                FirstAirDate = m.FirstAirDate,
                PosterPath = m.PosterPath
            })]
        };

        dbContext.Add(searchSnapshot);
        await dbContext.SaveChanges(ct);

        await messageRenderer.SendMediaPage(
                chatId: message.Chat.Id,
                media: searchResults[0],
                searchId: searchSnapshot.Id,
                currentIndex: 0,
                hasNext: searchResults.Count > 1,
                ct: ct);

        await BotClient.DeleteMessage(
            chatId: message.Chat.Id,
            messageId: loadingMessage.MessageId,
            cancellationToken: ct);
    }
}
