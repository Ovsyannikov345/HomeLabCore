namespace HomeServerTelegramWorker.Telegram.Handlers.CommandHandlers;

public interface ICommandHandler : ITelegramUpdateHandler
{
    public string CommandName { get; }

    public string CommandDescription { get; }

    public string? CommandExample { get; }
}
