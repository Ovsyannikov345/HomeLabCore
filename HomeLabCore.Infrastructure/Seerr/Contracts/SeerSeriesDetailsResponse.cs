using HomeLabCore.Infrastructure.Seerr.Constants.Enums;
using System.Text.Json.Serialization;

namespace HomeLabCore.Infrastructure.Seerr.Contracts;

internal sealed record SeerSeriesDetailsResponse
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("firstAirDate")]
    public string? FirstAirDate { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("numberOfSeasons")]
    public required int NumberOfSeasons { get; init; }

    [JsonPropertyName("overview")]
    public required string Overview { get; init; }

    [JsonPropertyName("seasons")]
    public required SeerSeasonInfo[] Seasons { get; init; }

    [JsonPropertyName("posterPath")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("mediaInfo")]
    public SeerSeriesMetadata? Metadata { get; init; }
}

internal sealed record SeerSeasonInfo
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("overview")]
    public required string Overview { get; init; }

    [JsonPropertyName("seasonNumber")]
    public required int SeasonNumber { get; init; }

    [JsonPropertyName("episodeCount")]
    public required int EpisodeCount { get; init; }
}

internal sealed record SeerSeriesMetadata
{
    [JsonPropertyName("seasons")]
    public required SeerSeasonMetadata[] Seasons { get; init; }
}

internal sealed record SeerSeasonMetadata
{
    [JsonPropertyName("seasonNumber")]
    public required int SeasonNumber { get; init; }

    [JsonPropertyName("status")]
    public required SeerrMediaStatus Status { get; init; }
}
