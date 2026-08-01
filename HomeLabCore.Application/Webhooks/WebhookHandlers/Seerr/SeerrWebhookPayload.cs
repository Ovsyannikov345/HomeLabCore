namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Seerr;

public sealed record SeerrWebhookPayload
{
    public required string NotificationType { get; init; }

    public required string Event { get; init; }

    public required string Subject { get; init; }

    public required string Message { get; init; }

    public WebhookMediaInfo? Media { get; init; }

    public WebhookMediaRequestInfo? Request { get; init; }

    public WebhookIssueInfo? Issue { get; init; }

    public WebhookCommentInfo? Comment { get; init; }
}

public sealed record WebhookMediaInfo
{
    public required string MediaType { get; init; }

    public required string TmdbId { get; init; }

    public required string TvdbId { get; init; }

    public required string Status { get; init; }
}

public sealed record WebhookMediaRequestInfo
{
    public required string RequestId { get; init; }

    public required string RequestedByEmail { get; init; }

    public required string RequestedByUsername { get; init; }

    public required string RequestedByAvatar { get; init; }

    public required string RequestedBySettingsDiscordId { get; init; }

    public required string RequestedBySettingsTelegramChatId { get; init; }
}

public sealed record WebhookIssueInfo
{
    public required string IssueId { get; init; }

    public required string IssueType { get; init; }

    public required string IssueStatus { get; init; }

    public required string ReportedByEmail { get; init; }

    public required string ReportedByUsername { get; init; }

    public required string ReportedByAvatar { get; init; }

    public required string ReportedBySettingsDiscordId { get; init; }

    public required string ReportedBySettingsTelegramChatId { get; init; }
}

public sealed record WebhookCommentInfo
{
    public required string CommentMessage { get; init; }

    public required string CommentedByEmail { get; init; }

    public required string CommentedByUsername { get; init; }

    public required string CommentedByAvatar { get; init; }

    public required string CommentedBySettingsDiscordId { get; init; }

    public required string CommentedBySettingsTelegramChatId { get; init; }
}
