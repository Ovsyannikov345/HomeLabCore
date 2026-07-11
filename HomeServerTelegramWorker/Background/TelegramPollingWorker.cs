using HomeServerTelegramWorker.Configuration;
using HomeServerTelegramWorker.Extensions;
using HomeServerTelegramWorker.Telegram.Handlers;
using HomeServerTelegramWorker.Telegram.Handlers.CommandHandlers;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeServerTelegramWorker.Background;

public sealed class TelegramPollingWorker(
    IServiceScopeFactory scopeFactory,
    ITelegramBotClient telegramBotClient,
    IOptionsMonitor<TelegramSettings> options,
    ILogger<TelegramPollingWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
            DropPendingUpdates = false
        };

        try
        {
            await telegramBotClient.ReceiveAsync(
                updateHandler: HandleUpdate,
                errorHandler: HandlePollingError,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Stopping polling because application is shutting down.");
        }
    }

    private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        long? chatId = update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id,
            _ => null
        };

        if (chatId is null)
        {
            logger.LogWarning("Unauthorized access attempt from unknown chat.");

            return;
        }

        if (!options.CurrentValue.ChatIdWhitelist.Any(id => id == chatId.Value))
        {
            logger.LogWarning("Unauthorized access attempt from Chat ID: {ChatId}", chatId);

            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();

        try
        {
            if (update.IsCommand())
            {
                await HandleCommand();
            }
            else
            {
                logger.LogError("Update {UpdateId} can't be handled because of unknown update type.", update.Id);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update {UpdateId}", update.Id);
        }

        async Task HandleCommand()
        {
            var commandHandlers = scope.ServiceProvider.GetServices<ICommandHandler>();

            if (commandHandlers.FirstOrDefault(h => h.CanHandle(update)) is { } capableHandler)
            {
                await capableHandler.Handle(update, ct);
            }
            else
            {
                var fallbackHandler = scope.ServiceProvider.GetRequiredService<IFallbackHandler>();

                await fallbackHandler.Handle(update, ct);
            }
        }
    }

    private Task HandlePollingError(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Telegram API Polling Error");

        return Task.CompletedTask;
    }
}
