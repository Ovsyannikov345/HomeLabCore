using HomeLabCore.Application.Telegram.Constants;
using System.Diagnostics.CodeAnalysis;

namespace HomeLabCore.Application.Telegram.CallbackQueryHandlers.Payloads;

public sealed record ChangeSearchPagePayload(Guid SearchId, int NextIndex) : ICallbackQueryPayload<ChangeSearchPagePayload>
{
    public string ToCallbackQueryString()
    {
        var prefix = CallbackQueryConstants.Prefixes.ChangePage;

        var delimiter = CallbackQueryConstants.Delimiter;

        return $"{prefix}{delimiter}{SearchId:N}{delimiter}{NextIndex}";
    }

    public static bool TryParse(string data, [NotNullWhen(true)] out ChangeSearchPagePayload? payload)
    {
        payload = null;

        var parts = data.Split(CallbackQueryConstants.Delimiter);

        if (parts.Length == 3 &&
            parts[0] == CallbackQueryConstants.Prefixes.ChangePage &&
            Guid.TryParseExact(parts[1], "N", out var searchId) &&
            int.TryParse(parts[2], out var index))
        {
            payload = new ChangeSearchPagePayload(searchId, index);

            return true;
        }

        return false;
    }
}
