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

    public bool CanRender(MediaRenderingPayload mediaPayload) => mediaPayload is MovieRenderingPayload;

    public TelegramMessage RenderMessage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext)
    {
        var moviePayload = (MovieRenderingPayload)mediaPayload;

        var caption = BuildCaption(moviePayload);

        var keyboard = BuildKeyboard(moviePayload, searchContext);

        var photo = GetPhoto(moviePayload);

        return new TelegramMessage()
        {
            Caption = caption,
            Keyboard = keyboard,
            Photo = photo
        };
    }

    private static string BuildCaption(MovieRenderingPayload moviePayload)
    {
        var caption = new StringBuilder();

        caption.AppendLine($"🎬 <b>{moviePayload.Title}{GetFormattedReleaseYear()}</b>");
        caption.AppendLine($"{MediaType.Movie.ToString().ToUpper()}\n");
        caption.AppendLine(moviePayload.Overview.Length > 800 ? $"{moviePayload.Overview.AsSpan(0, 800)}..." : moviePayload.Overview);

        return caption.ToString();

        string GetFormattedReleaseYear()
        {
            var releaseDate = moviePayload.ReleaseDate ?? moviePayload.FirstAirDate;

            if (string.IsNullOrWhiteSpace(releaseDate) || releaseDate.Length < 4)
            {
                return "";
            }

            return $" ({releaseDate.AsSpan(0, 4)})";
        }
    }

    private static InlineKeyboardMarkup BuildKeyboard(MovieRenderingPayload moviePayload, MediaSearchContext searchContext)
    {
        var actionRow = new InlineKeyboardButton[1];

        if (moviePayload.Status is MediaStatus.Available)
        {
            actionRow[0] = InlineKeyboardButton.WithCallbackData(
                "✅ Available",
                new EmptyPayload().ToCallbackQueryString());
        }
        else if (moviePayload.Status is MediaStatus.Pending or MediaStatus.Processing)
        {
            actionRow[0] = InlineKeyboardButton.WithCallbackData(
                "⏳ Processing...",
                new EmptyPayload().ToCallbackQueryString());
        }
        else
        {
            actionRow[0] = InlineKeyboardButton.WithCallbackData(
                "⬇️ Download",
                new RequestMediaPayload(MediaType.Movie, moviePayload.Id).ToCallbackQueryString());
        }

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

        var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>(2)
        {
            actionRow
        };

        if (navigationRow.Count > 0)
        {
            keyboardRows.Add(navigationRow);
        }

        return new InlineKeyboardMarkup(keyboardRows);
    }

    private static InputFile GetPhoto(MovieRenderingPayload moviePayload)
    {
        if (!string.IsNullOrWhiteSpace(moviePayload.PosterPath))
        {
            return InputFile.FromUri(moviePayload.PosterPath);
        }
        else
        {
            var fullPlaceholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PlaceholderImagePath);
            var fileStream = File.OpenRead(fullPlaceholderPath);

            return InputFile.FromStream(fileStream, Path.GetFileName(fullPlaceholderPath));
        }
    }
}
