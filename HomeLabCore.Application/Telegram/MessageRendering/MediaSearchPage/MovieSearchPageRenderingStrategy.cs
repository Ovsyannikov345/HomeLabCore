using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;
using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Domain.Constants.Enums;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;

internal sealed class MovieSearchPageRenderingStrategy : IMediaSearchPageRenderingStrategy
{
    // TODO Create provider for static assets
    private const string PlaceholderImagePath = "Assets/Images/no_image_placeholder.png";

    public bool CanRender(MediaType mediaType) => mediaType is MediaType.Movie;

    public TelegramMessage RenderMessage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var caption = BuildCaption(mediaPayload);

        var keyboard = BuildKeyboard(mediaPayload, searchContext);

        var photo = GetPhoto(mediaPayload);

        return new TelegramMessage()
        {
            Caption = caption,
            Keyboard = keyboard,
            Photo = photo
        };
    }

    private InputFile GetPhoto(MediaRenderingPayload mediaPayload)
    {
        if (!string.IsNullOrWhiteSpace(mediaPayload.PosterPath))
        {
            return InputFile.FromUri(mediaPayload.PosterPath);
        }
        else
        {
            var fullPlaceholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PlaceholderImagePath);
            var fileStream = File.OpenRead(fullPlaceholderPath);

            return InputFile.FromStream(fileStream, Path.GetFileName(fullPlaceholderPath));
        }
    }

    private string BuildCaption(MediaRenderingPayload mediaPayload)
    {
        var releaseDate = mediaPayload.ReleaseDate ?? mediaPayload.FirstAirDate;

        if (releaseDate is not null)
        {
            releaseDate = $" ({releaseDate[..4]})";
        }

        var caption = new StringBuilder();

        caption.AppendLine($"🎬 **{mediaPayload.Title}{releaseDate}**");
        caption.AppendLine($"*{mediaPayload.MediaType.ToString().ToUpper()}*\n");
        caption.AppendLine(mediaPayload.Overview.Length > 800 ? mediaPayload.Overview[..800] + "..." : mediaPayload.Overview);

        return caption.ToString();
    }

    private InlineKeyboardMarkup BuildKeyboard(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var row1 = new InlineKeyboardButton[1];

        if (mediaPayload.Status is MediaStatus.Available)
        {
            row1[0] = InlineKeyboardButton.WithCallbackData(
                "✅ Available",
                new EmptyPayload().ToCallbackQueryString());
        }
        else if (mediaPayload.Status is MediaStatus.Pending or MediaStatus.Processing)
        {
            row1[0] = InlineKeyboardButton.WithCallbackData(
                "⏳ Processing...",
                new EmptyPayload().ToCallbackQueryString());
        }
        else
        {
            row1[0] = InlineKeyboardButton.WithCallbackData(
                "⬇️ Download",
                new RequestMediaPayload(mediaPayload.MediaType, mediaPayload.Id).ToCallbackQueryString());
        }

        var row2 = new List<InlineKeyboardButton>();

        if (searchContext.CurrentIndex > 0)
        {
            row2.Add(InlineKeyboardButton.WithCallbackData(
                "⬅️ Previous",
                new ChangeSearchPagePayload(searchContext.SearchId, searchContext.CurrentIndex - 1).ToCallbackQueryString()));
        }

        if (searchContext.HasNext)
        {
            row2.Add(InlineKeyboardButton.WithCallbackData(
                "➡️ Next",
                new ChangeSearchPagePayload(searchContext.SearchId, searchContext.CurrentIndex + 1).ToCallbackQueryString()));
        }

        var keyboardRows = new List<IEnumerable<InlineKeyboardButton>> { row1 };

        if (row2.Count > 0)
        {
            keyboardRows.Add(row2);
        }

        return new InlineKeyboardMarkup(keyboardRows);
    }
}
