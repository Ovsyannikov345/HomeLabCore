using System.ComponentModel.DataAnnotations;

namespace HomeServerTelegramWorker;

public sealed record SeerrSettings
{
    public const string SectionName = "Seerr";

    [Required(AllowEmptyStrings = false)]
    public required string BaseUrl { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string ApiKey { get; init; }
}
