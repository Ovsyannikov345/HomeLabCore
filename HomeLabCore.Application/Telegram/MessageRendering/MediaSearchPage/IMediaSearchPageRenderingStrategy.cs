using HomeLabCore.Application.Telegram.Dto;
using HomeLabCore.Domain.Constants.Enums;

namespace HomeLabCore.Application.Telegram.MessageRendering.MediaSearchPage;

internal interface IMediaSearchPageRenderingStrategy
{
    public bool CanRender(MediaType mediaType);

    public TelegramMessage RenderMessage(MediaRenderingPayload mediaPayload, MediaSearchContext searchContext);
}

internal sealed record MediaRenderingPayload
{
    public int Id { get; init; }

    public required MediaType MediaType { get; init; }

    public required string Title { get; init; }

    public required string Overview { get; init; }

    public required MediaStatus Status { get; init; }

    public string? ReleaseDate { get; init; }

    public string? FirstAirDate { get; init; }

    public string? PosterPath { get; init; }
}

internal sealed record MediaSearchContext
{
    public required Guid SearchId { get; init; }

    public required int CurrentIndex { get; init; }

    public required bool HasNext { get; init; }
}
