using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeServerTelegramWorker.Extensions;

public static class TelegramUpdateExtensions
{
    public static bool IsCommand(this Update update)
    {
        return update.Type is UpdateType.Message
            && update.Message?.Text is not null
            && update.Message.Text.StartsWith('/');
    }
}
