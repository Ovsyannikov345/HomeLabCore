using HomeLabCore.Domain.Constants.Enums;
using Microsoft.Extensions.Logging;

namespace HomeLabCore.Application.Logging;

internal static partial class ApplicationLogs
{
    #region Callback Query Handlers

    [LoggerMessage(
        EventId = 3_0001_0001,
        Level = LogLevel.Warning,
        Message = "Unauthorized access attempt by UserId {UserId} for query {QueryPrefix}")]
    public static partial void CallbackQueryAccessDenied(this ILogger logger, long? userId, string queryPrefix);

    [LoggerMessage(
        EventId = 3_0001_0002,
        Level = LogLevel.Warning,
        Message = "Callback query rejected. Query ID: {CallbackQueryId}. Reason: {Reason}")]
    public static partial void CallbackQueryRejected(this ILogger logger, string callbackQueryId, string reason);

    [LoggerMessage(
        EventId = 3_0001_0003,
        Level = LogLevel.Warning,
        Message = "Failed to parse callback query data '{CallbackQueryData}' into payload type {PayloadType}")]
    public static partial void CallbackQueryParsingFailed(this ILogger logger, string callbackQueryData, string payloadType);

    [LoggerMessage(
        EventId = 3_0001_0004,
        Level = LogLevel.Information,
        Message = "Starting to process a callback query with data {CallbackQueryData} from user {UserId}")]
    public static partial void CallbackQueryProcessingStarted(this ILogger logger, string callbackQueryData, long? userId);

    [LoggerMessage(
        EventId = 3_0001_0005,
        Level = LogLevel.Information,
        Message = "Callback query processed successfully")]
    public static partial void CallbackQueryProcessingFinished(this ILogger logger);

    [LoggerMessage(
        EventId = 3_0001_0006,
        Level = LogLevel.Warning,
        Message = "Error occurred while processing callback query {CallbackQueryId} from UserId {UserId}. Reason: {Reason}")]
    public static partial void CallbackQueryProcessingFailed(this ILogger logger, string callbackQueryId, long? userId, string reason);

    [LoggerMessage(
        EventId = 3_0001_0007,
        Level = LogLevel.Error,
        Message = "Error occurred while processing callback query {CallbackQueryId} from UserId {UserId}")]
    public static partial void CallbackQueryProcessingFailed(this ILogger logger, string callbackQueryId, long? userId, Exception ex);

    [LoggerMessage(
        EventId = 3_0001_0008,
        Level = LogLevel.Information,
        Message = "Responded to callback query {CallbackQueryId} for ChatId {ChatId} with a new message")]
    public static partial void RespondedToCallbackQueryWithNewMessage(this ILogger logger, string callbackQueryId, long? chatId);

    [LoggerMessage(
        EventId = 3_0001_0009,
        Level = LogLevel.Information,
        Message = "Updated an inline keyboard for message {MessageId}")]
    public static partial void UpdatedInlineKeyboardForMessage(this ILogger logger, long? messageId);

    [LoggerMessage(
        EventId = 3_0001_0010,
        Level = LogLevel.Information,
        Message = "Deleted message {MessageId}")]
    public static partial void DeletedMessage(this ILogger logger, long? messageId);

    [LoggerMessage(
        EventId = 3_0001_0011,
        Level = LogLevel.Warning,
        Message = "Failed to fetch latest media status. User will be presented with a snapshot data. Media Type: {MediaType}. Media ID: {MediaId}")]
    public static partial void FailedToFetchLatestMediaStatus(this ILogger logger, MediaType mediaType, int mediaId);

    [LoggerMessage(
        EventId = 3_0001_0012,
        Level = LogLevel.Error,
        Message = "Failed to request media. Media Type: {MediaType}. Media ID: {MediaId}")]
    public static partial void FailedToRequestMedia(this ILogger logger, MediaType mediaType, int mediaId, Exception ex);

    [LoggerMessage(
        EventId = 3_0001_0013,
        Level = LogLevel.Information,
        Message = "Movie requested successfully. Media ID: {MediaId}")]
    public static partial void RequestedMovie(this ILogger logger, int mediaId);

    [LoggerMessage(
        EventId = 3_0001_0014,
        Level = LogLevel.Information,
        Message = "Series requested successfully. Media ID: {MediaId}. Season number: {SeasonNumber}")]
    public static partial void RequestedSeries(this ILogger logger, int mediaId, int seasonNumber);

    #endregion

    #region Command Handlers

    [LoggerMessage(
        EventId = 3_0002_0001,
        Level = LogLevel.Warning,
        Message = "Unauthorized access attempt by UserId {UserId} for command {CommandName}")]
    public static partial void CommandAccessDenied(this ILogger logger, long? userId, string commandName);

    [LoggerMessage(
        EventId = 3_0002_0002,
        Level = LogLevel.Information,
        Message = "Starting to process a command with text {CommandText} from user {UserId} in chat {ChatId}")]
    public static partial void CommandProcessingStarted(this ILogger logger, string? commandText, long? userId, long chatId);

    [LoggerMessage(
        EventId = 3_0002_0003,
        Level = LogLevel.Information,
        Message = "Command processed successfully")]
    public static partial void CommandProcessingFinished(this ILogger logger);

    [LoggerMessage(
        EventId = 3_0002_0004,
        Level = LogLevel.Warning,
        Message = "Command processing failed. Reason: {Reason}")]
    public static partial void CommandProcessingFailed(this ILogger logger, string reason);

    [LoggerMessage(
        EventId = 3_0002_0005,
        Level = LogLevel.Error,
        Message = "Command processing failed")]
    public static partial void CommandProcessingFailed(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 3_0002_0006,
        Level = LogLevel.Information,
        Message = "Responded to command in ChatId {ChatId} with a new message")]
    public static partial void RespondedToCommandWithNewMessage(this ILogger logger, long? chatId);

    [LoggerMessage(
        EventId = 3_0002_0007,
        Level = LogLevel.Information,
        Message = "Updated text for message {MessageId}")]
    public static partial void UpdatedTextForMessage(this ILogger logger, long? messageId);

    #endregion

    #region Background Processing

    [LoggerMessage(
        EventId = 3_0003_0001,
        Level = LogLevel.Error,
        Message = "Background processing message is dropped. Reason: {Reason}")]
    public static partial void BackgroundProcessingMessageDropped(this ILogger logger, string reason);

    [LoggerMessage(
        EventId = 3_0003_0002,
        Level = LogLevel.Information,
        Message = "Background processing message is sent")]
    public static partial void BackgroundProcessingMessageSent(this ILogger logger);

    #endregion

    #region Webhook Processing

    [LoggerMessage(
        EventId = 3_0004_0001,
        Level = LogLevel.Error,
        Message = "Failed to deserialize webhook payload. Raw payload: {RawPayload}")]
    public static partial void FailedToDeserializeWebhook(this ILogger logger, string rawPayload, Exception ex);

    [LoggerMessage(
        EventId = 3_0004_0002,
        Level = LogLevel.Error,
        Message = "Failed to handle webhook. Reason: {Reason}")]
    public static partial void FailedToHandleWebhook(this ILogger logger, string reason);

    [LoggerMessage(
        EventId = 3_0004_0003,
        Level = LogLevel.Error,
        Message = "Failed to handle webhook")]
    public static partial void FailedToHandleWebhook(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 3_0004_0004,
        Level = LogLevel.Information,
        Message = "Webhook processed successfully")]
    public static partial void WebhookProcessed(this ILogger logger);

    [LoggerMessage(
        EventId = 3_0004_0005,
        Level = LogLevel.Information,
        Message = "Seerr test notification received")]
    public static partial void SeerrTestNotificationReceived(this ILogger logger);

    [LoggerMessage(
        EventId = 3_0004_0006,
        Level = LogLevel.Warning,
        Message = "Seerr unknown notification received. Notification type: {NotificationType}")]
    public static partial void SeerrUnknownNotificationReceived(this ILogger logger, string? notificationType);

    #endregion

    #region Media Notifications

    [LoggerMessage(
        EventId = 3_0005_0001,
        Level = LogLevel.Warning,
        Message = "Failed to subscribe to media notifications")]
    public static partial void FailedToSubscribeToMedia(this ILogger logger, Exception ex);

    #endregion
}
