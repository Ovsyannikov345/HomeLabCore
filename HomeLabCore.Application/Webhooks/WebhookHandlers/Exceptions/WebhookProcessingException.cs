namespace HomeLabCore.Application.Webhooks.WebhookHandlers.Exceptions;

public sealed class WebhookProcessingException(string message) : Exception(message)
{
}
