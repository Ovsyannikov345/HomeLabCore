using HomeLabCore.Application.Telegram.Constants;
using HomeLabCore.Domain.Constants.Enums;
using System.Diagnostics.CodeAnalysis;

namespace HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;

public sealed record RequestMediaPayload(
    MediaType MediaType, 
    int MediaId, 
    int? SeasonNumber) 
    : ICallbackQueryPayload<RequestMediaPayload>
{
    public bool IsMovie => MediaType is MediaType.Movie;

    [MemberNotNullWhen(true, nameof(SeasonNumber))]
    public bool IsSeries => MediaType is MediaType.Series && SeasonNumber.HasValue;

    public static bool TryParse(string data, [NotNullWhen(true)] out RequestMediaPayload? payload)
    {
        payload = null;

        var parts = data.Split(CallbackQueryConstants.Delimiter);

        if (parts.Length < 3
            || parts[0] != CallbackQueryConstants.Prefixes.RequestMedia
            || !Enum.TryParse(parts[1], out MediaType type)
            || !int.TryParse(parts[2], out var id))
        {
            return false;
        }

        if (type is MediaType.Movie)
        {
            payload = new RequestMediaPayload(type, id, null);

            return true;
        }

        if (parts.Length == 4 && int.TryParse(parts[3], out var seasonNumber))
        {
            payload = new RequestMediaPayload(type, id, seasonNumber);

            return true;
        }

        return false;
    }

    public string ToCallbackQueryString()
    {
        var prefix = CallbackQueryConstants.Prefixes.RequestMedia;

        var delimiter = CallbackQueryConstants.Delimiter;

        return $"{prefix}{delimiter}{MediaType}{delimiter}{MediaId}{delimiter}{SeasonNumber}";
    }
}
