using HomeLabCore.Application.Interfaces.Clients;
using HomeLabCore.Application.Logging;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Abstractions;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;
using HomeLabCore.Application.Telegram.Configuration;
using HomeLabCore.Application.Telegram.Constants;
using HomeLabCore.Application.Telegram.Exceptions;
using HomeLabCore.Application.Telegram.MessageRendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace HomeLabCore.Application.Telegram.CallbackQueryHandlers;

internal sealed class RequestMediaQueryHandler(
    ITelegramBotClient telegramBotClient,
    IMediaManagerClient mediaManagerClient,
    IMessageRenderer messageRenderer,
    IOptionsSnapshot<TelegramSettings> options,
    ILogger<RequestMediaQueryHandler> logger)
    : CallbackQueryHandlerBase<RequestMediaPayload>(telegramBotClient, options, logger)
{
    protected override string QueryPrefix => CallbackQueryConstants.Prefixes.RequestMedia;

    protected override bool RequiresAuthorization => true;

    protected override async Task ProcessCallbackQuery(CallbackQueryContext context, RequestMediaPayload payload, CancellationToken ct)
    {
        try
        {
            if (payload.IsMovie)
            {
                await mediaManagerClient.RequestMovie(payload.MediaId, ct);

                Logger.RequestedMovie(payload.MediaId);
            }
            else if (payload.IsSeries)
            {
                await mediaManagerClient.RequestSeries(payload.MediaId, [payload.SeasonNumber.Value], ct);

                Logger.RequestedSeries(payload.MediaId, payload.SeasonNumber.Value);
            }

        }
        catch (Exception ex) when (ex is not (OperationCanceledException or CallbackQueryProcessingException))
        {
            Logger.FailedToRequestMedia(payload.MediaType, payload.MediaId, ex);

            throw new CallbackQueryProcessingException("Failed to request media", showToUser: true);
        }

        var keyboard = context.SourceMessage.ReplyMarkup;

        if (keyboard is null)
        {
            return;
        }

        var updatedKeyboard = messageRenderer.RenderKeyboardAfterRequest(keyboard, payload.MediaType, payload.SeasonNumber);

        await UpdateMessageKeyboard(updatedKeyboard, ct);
    }
}
