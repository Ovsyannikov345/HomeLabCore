using HomeLabCore.Api.Configuration;
using HomeLabCore.Api.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HomeLabCore.Api.Authorization;

public class ApiKeyAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    IOptionsMonitor<ApiKeyAuthenticationSettings> apiKeyAuthOptions,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.ApiKey, out var extractedApiKey))
        {
            Logger.LogInformation("API Key was not provided.");

            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var providedApiKey = extractedApiKey.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(providedApiKey) ||
            !apiKeyAuthOptions.CurrentValue.ValidKeys.Contains(providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "ApiKeyUser"),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
