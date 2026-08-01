using HomeLabCore.Domain.Constants.Enums;

namespace HomeLabCore.Domain.Entities.Media;

public sealed class MediaSubscription : EntityBase
{
    public long UserId { get; set; }

    public required MediaType MediaType { get; set; }

    public required int MediaExternalId { get; set; }
}
