using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;
using HomeLabCore.Application.Webhooks.WebhookHandlers.Seerr;
using HomeLabCore.Domain.Constants.Enums;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeLabCore.Application.Telegram.MessageRendering;

internal interface IMessageRenderer
{
    public TelegramMessage RenderMediaSearchPage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext);

    public InlineKeyboardMarkup RenderKeyboardAfterRequest(InlineKeyboardMarkup keyboard, MediaType mediaType, int? requestedSeason);

    public TelegramMessage RenderSeerrNotification(SeerrWebhookPayload payload);
}

internal class MessageRenderer(IEnumerable<IMediaSearchPageRenderingStrategy> searchPageStrategies) 
    : IMessageRenderer
{
    public TelegramMessage RenderMediaSearchPage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var renderingStrategy = searchPageStrategies.First(r => r.CanRender(mediaPayload));

        return renderingStrategy.RenderMessage(mediaPayload, searchContext);
    }

    public InlineKeyboardMarkup RenderKeyboardAfterRequest(InlineKeyboardMarkup keyboard, MediaType mediaType, int? requestedSeason)
    {
        var renderingStrategy = searchPageStrategies.First(r => r.CanRenderKeyboard(mediaType));

        return renderingStrategy.RenderKeyboardAfterRequest(keyboard, requestedSeason);
    }

    public TelegramMessage RenderSeerrNotification(SeerrWebhookPayload payload)
    {
        var caption = new StringBuilder();

        caption.AppendLine($"🔔 <b>{payload.Subject}</b>\n");
        caption.AppendLine(payload.Message);

        return new TelegramMessage()
        {
            Caption = caption.ToString(),
            Keyboard = null,
            Photo = null
        };
    }
}
