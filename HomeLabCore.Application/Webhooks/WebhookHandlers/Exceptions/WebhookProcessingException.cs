namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Exceptions;

internal sealed class WebhookProcessingException(string message) : Exception(message)
{
}
