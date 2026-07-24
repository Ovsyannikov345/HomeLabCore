using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Domain.Constants.Enums;

namespace HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;

internal interface IMediaSearchPageRenderingStrategy
{
    public bool CanRender(MediaRenderingPayload mediaPayload);

    public TelegramMessage RenderMessage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext);
}

internal abstract record MediaRenderingPayload
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required string Overview { get; init; }

    public string? FirstAirDate { get; init; }

    public string? PosterPath { get; init; }
}

internal sealed record MovieRenderingPayload : MediaRenderingPayload
{
    public string? ReleaseDate { get; init; }

    public required MediaStatus Status { get; init; }
}

internal sealed record SeriesRenderingPayload : MediaRenderingPayload
{
    public required Season[] Seasons { get; init; }

    internal sealed record Season
    {
        public required int Id { get; init; }

        public required int Number { get; init; }

        public required MediaStatus Status { get; init; }
    }
}

internal sealed record MediaSearchContext
{
    public required Guid SearchId { get; init; }

    public required int CurrentIndex { get; init; }

    public required bool HasNext { get; init; }
}
