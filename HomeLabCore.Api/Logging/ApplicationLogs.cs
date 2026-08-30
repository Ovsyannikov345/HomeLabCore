using HomeLabCore.Application.Constants.Enums;

namespace HomeLabCore.Api.Logging;

internal static partial class ApplicationLogs
{
    #region Middleware

    [LoggerMessage(
        EventId = 4_0001_0001,
        Level = LogLevel.Information,
        Message = "Received correlation ID '{ReceivedCorrelationId}' is empty or invalid. Generated new correlation ID: '{GeneratedCorrelationId}'")]
    public static partial void GeneratedNewRequestCorrelationId(this ILogger logger, string? receivedCorrelationId, string generatedCorrelationId);

    #endregion

    #region Webhooks

    [LoggerMessage(
        EventId = 4_0002_0001,
        Level = LogLevel.Information,
        Message = "Received new webhook from {WebhookSender}. Raw payload: {WebhookPayload}'")]
    public static partial void WebhookReceived(this ILogger logger, WebhookSender webhookSender, string webhookPayload);

    #endregion
}
