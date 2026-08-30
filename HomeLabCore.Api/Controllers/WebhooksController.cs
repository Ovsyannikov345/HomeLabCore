using HomeLabCore.Api.Constants;
using HomeLabCore.Api.Logging;
using HomeLabCore.Application.Background.Webhooks;
using HomeLabCore.Application.Constants.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLabCore.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public sealed class WebhooksController(IWebhookEventWriter webhookEventWriter, ILogger<WebhooksController> logger) : ControllerBase
{
    [HttpPost("seerr")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.ApiKey)]
    public async Task<ActionResult> ProcessSeerrWebhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);

        var payload = await reader.ReadToEndAsync(ct);

        if (string.IsNullOrWhiteSpace(payload))
        {
            return BadRequest("Webhook payload cannot be empty.");
        }

        logger.WebhookReceived(WebhookSender.Seerr, payload);

        webhookEventWriter.Write(payload, WebhookSender.Seerr);

        return Accepted();
    }
}
