using HomeLabCore.Domain.Constants.Enums;

namespace HomeLabCore.Application.Dto.Media;

public sealed record ExternalSeriesDetails
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Overview { get; init; }

    public string? FirstAirDate { get; init; }

    public string? PosterPath { get; init; }

    public required int NumberOfSeasons { get; init; }

    public required ExternalSeriesSeasonInfo[] Seasons { get; init; }
}

public sealed record ExternalSeriesSeasonInfo
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Overview { get; init; }

    public required int SeasonNumber { get; init; }

    public required int EpisodeCount { get; init; }

    public required MediaStatus Status { get; init; }
}
