using Telegram.Bot.Types;

namespace HomeServerTelegramWorker.Telegram.Handlers;

public interface ITelegramUpdateHandler
{
    public bool CanHandle(Update update);

    public Task Handle(Update update, CancellationToken ct);
}
