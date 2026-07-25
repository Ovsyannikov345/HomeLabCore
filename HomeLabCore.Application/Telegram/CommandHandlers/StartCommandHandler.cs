using HomeLabCore.Application.Telegram.CommandHandlers.Abstractions;
using HomeLabCore.Application.Telegram.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeLabCore.Application.Telegram.CommandHandlers;

internal sealed class StartCommandHandler(
    ITelegramBotClient telegramBotClient,
    IOptionsSnapshot<TelegramSettings> options,
    ILogger<StartCommandHandler> logger)
    : CommandHandlerBase(telegramBotClient, options, logger)
{
    public override CommandHandlerOptions HandlerOptions => new()
    {
        RequiresAuthorization = false,
        CommandName = "start",
        CommandDescription = "Starts a conversation with the bot",
        CommandExample = null
    };

    protected override async Task ProcessUpdate(Message message, Message botResponseMessage, CancellationToken ct)
    {
        var userId = message.From?.Id;

        var chatId = message.Chat.Id;

        var firstName = message.From?.FirstName ?? "anonymous user";

        if (userId is not null && Settings.UserIdWhitelist.Contains(userId.Value))
        {
            var alreadyWhitelistedText = $"""
            👋 <b>Welcome, {firstName}!</b>

            You're already whitelisted. Anyways, here are your ID's:

            👤 <b>User ID:</b> `{userId}`
            💬 <b>Chat ID:</b> `{chatId}`

            <i>Use /help command to get started.</i>
            """;

            await RespondWithText(alreadyWhitelistedText, ct);

            return;
        }

        var greetingText = $"""
        👋 <b>Welcome, {firstName}!</b>

        This is a private home server bot. To use it, you must be authorized by the administrator.

        Please tap the IDs below to copy them, and send them to the admin to be whitelisted:

        👤 <b>User ID:</b> `{userId}`
        💬 <b>Chat ID:</b> `{chatId}`

        <i>Once the admin confirms you are added, you can start talking to me!<i>
        <i>Use /help command to get started.<i>
        """;

        await RespondWithText(greetingText, ct);
    }
}
