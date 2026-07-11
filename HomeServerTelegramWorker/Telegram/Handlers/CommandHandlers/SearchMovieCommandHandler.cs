using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeServerTelegramWorker.Telegram.Handlers.CommandHandlers;

public sealed class SearchMovieCommandHandler(
    ITelegramBotClient telegramBotClient,
    ILogger<SearchMovieCommandHandler> logger) 
    : CommandHandlerBase, ICommandHandler
{
    public override string CommandName => "movie";

    public override string CommandDescription => "Searches the requested movie";

    public override string? CommandExample => $"/{CommandName} The Matrix";

    protected override async Task ProcessUpdate(Message message, CancellationToken ct)
    {
        var searchTerm = GetCommandArgument(message);

        if (searchTerm is null)
        {
            await telegramBotClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Please provide a movie name. Example: `{CommandExample}`",
                parseMode: ParseMode.Markdown,
                cancellationToken: ct);

            return;
        }

        searchTerm = searchTerm.Trim();

        // TODO proper logging
        logger.LogInformation("Chat {ChatId} searching for: {Query}", message.Chat.Id, searchTerm);

        await telegramBotClient.SendMessage(
            chatId: message.Chat.Id,
            text: $"Searching Overseerr for movie: {searchTerm}...",
            cancellationToken: ct);
    }
}
