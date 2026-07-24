using HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;
using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Domain.Constants.Enums;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;

internal sealed class SeriesSearchPageRenderingStrategy : IMediaSearchPageRenderingStrategy
{
    // TODO Create provider for static assets
    private const string PlaceholderImagePath = "Assets/Images/no_image_placeholder.png";

    public bool CanRender(MediaRenderingPayload mediaPayload) => mediaPayload is SeriesRenderingPayload;

    public TelegramMessage RenderMessage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var seriesPayload = (SeriesRenderingPayload)mediaPayload;

        var caption = BuildCaption(seriesPayload);

        var keyboard = BuildKeyboard(seriesPayload, searchContext);

        var photo = GetPhoto(seriesPayload);

        return new TelegramMessage()
        {
            Caption = caption,
            Keyboard = keyboard,
            Photo = photo
        };
    }

    private static string BuildCaption(SeriesRenderingPayload seriesPayload)
    {
        var releaseDate = seriesPayload.FirstAirDate;

        if (releaseDate is not null)
        {
            releaseDate = $" ({releaseDate[..4]})";
        }

        var caption = new StringBuilder();

        caption.AppendLine($"📺 <b>{seriesPayload.Title}{releaseDate}</b>");
        caption.AppendLine($"{MediaType.Series.ToString().ToUpper()} - {seriesPayload.Seasons.Length} seasons\n");
        caption.AppendLine(seriesPayload.Overview.Length > 800 ? seriesPayload.Overview[..800] + "..." : seriesPayload.Overview);

        return caption.ToString();
    }

    private static InlineKeyboardMarkup BuildKeyboard(SeriesRenderingPayload seriesPayload, MediaSearchContext searchContext)
    {
        var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>(seriesPayload.Seasons.Length + 1);

        // Build action buttons for each season
        foreach (var season in seriesPayload.Seasons.Where(s => s.Number != 0).OrderBy(s => s.Number))
        {
            var seasonActionRow = new InlineKeyboardButton[1];

            seasonActionRow[0] = season.Status switch
            {
                MediaStatus.Available =>
                    InlineKeyboardButton.WithCallbackData(
                        $"Season {season.Number} - ✅ Available",
                        new EmptyPayload().ToCallbackQueryString()),

                MediaStatus.Pending or MediaStatus.Processing =>
                    InlineKeyboardButton.WithCallbackData(
                        $"Season {season.Number} - ⏳ Processing...",
                        new EmptyPayload().ToCallbackQueryString()),

                _ => InlineKeyboardButton.WithCallbackData(
                    $"Season {season.Number} - ⬇️ Download",
                    new RequestMediaPayload(MediaType.Series, seriesPayload.Id).ToCallbackQueryString())
            };

            keyboardRows.Add(seasonActionRow);
        }

        // Build navigation buttons
        var navigationRow = new List<InlineKeyboardButton>();

        if (searchContext.CurrentIndex > 0)
        {
            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                "⬅️ Previous",
                new ChangeSearchPagePayload(searchContext.SearchId, searchContext.CurrentIndex - 1).ToCallbackQueryString()));
        }

        if (searchContext.HasNext)
        {
            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                "➡️ Next",
                new ChangeSearchPagePayload(searchContext.SearchId, searchContext.CurrentIndex + 1).ToCallbackQueryString()));
        }

        if (navigationRow.Count > 0)
        {
            keyboardRows.Add(navigationRow);
        }

        return new InlineKeyboardMarkup(keyboardRows);
    }

    private static InputFile GetPhoto(SeriesRenderingPayload seriesPayload)
    {
        if (!string.IsNullOrWhiteSpace(seriesPayload.PosterPath))
        {
            return InputFile.FromUri(seriesPayload.PosterPath);
        }
        else
        {
            var fullPlaceholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PlaceholderImagePath);
            var fileStream = File.OpenRead(fullPlaceholderPath);

            return InputFile.FromStream(fileStream, Path.GetFileName(fullPlaceholderPath));
        }
    }
}
