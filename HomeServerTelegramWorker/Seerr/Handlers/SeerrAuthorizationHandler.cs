using Microsoft.Extensions.Options;

namespace HomeServerTelegramWorker.Seerr.Handlers;

public class SeerrAuthorizationHandler(IOptionsMonitor<SeerrSettings> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var settings = options.CurrentValue;

        request.Headers.Add("X-Api-Key", settings.ApiKey);

        return await base.SendAsync(request, cancellationToken);
    }
}
