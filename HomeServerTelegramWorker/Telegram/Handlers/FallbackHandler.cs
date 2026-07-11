using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeServerTelegramWorker.Telegram.Handlers;

public interface IFallbackHandler
{
    public Task Handle(Update update, CancellationToken ct);
}

public sealed class FallbackHandler(ITelegramBotClient telegramBotClient, ILogger<FallbackHandler> logger) : IFallbackHandler
{
    public async Task Handle(Update update, CancellationToken ct)
    {
        logger.LogWarning("No handler found for update {UpdateId}. Sending fallback message.", update.Id);

        if (update.Message is not null)
        {
            await telegramBotClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Sorry, I don't know how to handle that. Use /help command to see the list of available commands.",
                cancellationToken: ct
            );
        }
    }
}
