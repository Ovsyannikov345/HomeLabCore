namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Seerr;

internal static class SeerNotificationTypes
{
    public const string TestNotification = "TEST_NOTIFICATION";
    
    public const string MediaPending = "MEDIA_PENDING";
    
    public const string MediaApproved = "MEDIA_APPROVED";
    
    public const string MediaAvailable = "MEDIA_AVAILABLE";
    
    public const string MediaFailed = "MEDIA_FAILED";
    
    public const string MediaDeclined = "MEDIA_DECLINED";

    public const string MediaAutoRequested = "MEDIA_AUTO_REQUESTED";

    public const string MediaAutoApproved = "MEDIA_AUTO_APPROVED";
    
    public const string IssueCreated = "ISSUE_CREATED";
    
    public const string IssueComment = "ISSUE_COMMENT";
    
    public const string IssueResolved = "ISSUE_RESOLVED";
    
    public const string IssueReopened = "ISSUE_REOPENED";
}
