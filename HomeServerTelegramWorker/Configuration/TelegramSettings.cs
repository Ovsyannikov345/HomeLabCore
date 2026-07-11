using System.ComponentModel.DataAnnotations;

namespace HomeServerTelegramWorker.Configuration;

public sealed record TelegramSettings
{
    public const string SectionName = "Telegram";

    [Required(AllowEmptyStrings = false)]
    public required string BotToken { get; init; }

    public required long[] ChatIdWhitelist { get; init; }
}
