using HomeServerTelegramWorker.Seerr.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace HomeServerTelegramWorker.Seerr;

public interface ISeerrClient
{
    public Task<List<SeerrMedia>> Search(string query, CancellationToken ct);
}

public sealed class SeerrClient(HttpClient httpClient, IOptions<SeerrSettings> options) : ISeerrClient
{
    private readonly SeerrSettings _settings = options.Value;

    public async Task<List<SeerrMedia>> Search(string query, CancellationToken ct)
    {
        var requestUrl = QueryHelpers.AddQueryString($"{_settings.BaseUrl}/api/v1/search", new Dictionary<string, string?>
        {
            ["query"] = query,
        });

        var response = await httpClient.GetAsync(requestUrl, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SeerrSearchResponse>(cancellationToken: ct);

        return result?.Results.Where(m => m.MediaType is "movie" or "tv").ToList() ?? [];
    }
}
