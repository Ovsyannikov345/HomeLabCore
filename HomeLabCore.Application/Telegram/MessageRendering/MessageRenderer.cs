using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;

namespace HomeLabCore.Application.Telegram.MessageRendering;

internal interface IMessageRenderer
{
    public TelegramMessage RenderMediaSearchPage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext);
}

internal class MessageRenderer(IEnumerable<IMediaSearchPageRenderingStrategy> searchPageStrategies) 
    : IMessageRenderer
{
    public TelegramMessage RenderMediaSearchPage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var renderingStrategy = searchPageStrategies.First(r => r.CanRender(mediaPayload));

        return renderingStrategy.RenderMessage(mediaPayload, searchContext);
    }
}
