using HomeLabCore.Infrastructure.Seerr.Configuration;
using HomeLabCore.Infrastructure.Seerr.Constants;
using Microsoft.Extensions.Options;

namespace HomeLabCore.Infrastructure.Seerr.Handlers;

internal sealed class SeerrAuthorizationHandler(IOptionsMonitor<SeerrSettings> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var settings = options.CurrentValue;

        request.Headers.Add(ApiHeaderNames.ApiKey, settings.ApiKey);

        return await base.SendAsync(request, cancellationToken);
    }
}
