using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeLabCore.Application.Telegram.CommandHandlers;

public interface ICommandHandler
{
    public string CommandName { get; }

    public string CommandDescription { get; }

    public string? CommandExample { get; }

    public bool CanHandle(Message message);

    public Task Handle(Message message, CancellationToken ct);
}

public abstract class CommandHandlerBase(ITelegramBotClient telegramBotClient) : ICommandHandler
{
    protected ITelegramBotClient BotClient = telegramBotClient;

    public abstract string CommandName { get; }

    public abstract string CommandDescription { get; }

    public abstract string? CommandExample { get; }

    public virtual bool CanHandle(Message message)
    {
        if (message.Text is null
            || !message.Text.StartsWith('/'))
        {
            return false;
        }

        var parts = message.Text[1..].Split(' ');

        // Remove bot name if present (e.g., /command@botname)
        var command = parts[0].Split('@')[0];

        if (!string.Equals(command, CommandName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    // TODO logs here
    public async Task Handle(Message message, CancellationToken ct)
    {
        try
        {
            await ProcessUpdate(message, ct);
        }
        // TODO add correrlation id to message for debugging
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await BotClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Something went wrong while processing the command :(",
                parseMode: ParseMode.Markdown,
                cancellationToken: ct);
        }
    }

    protected abstract Task ProcessUpdate(Message message, CancellationToken ct);

    protected static string? GetCommandArgument(Message message)
    {
        var parts = message.Text?.Split(' ', 2);

        if (parts is null || parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
        {
            return null;
        }

        return parts[1];
    }
}
