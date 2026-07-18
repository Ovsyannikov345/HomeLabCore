using HomeLabCore.Api.Constants;
using HomeLabCore.Shared.Constants;
using HomeLabCore.Shared.Contexts;
using Serilog.Context;

namespace HomeLabCore.Api.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var receivedCorrelationIds);

        var correlationId = receivedCorrelationIds.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId) || !Guid.TryParse(correlationId, out _))
        {
            // TODO log here

            correlationId = CorrelationContext.CorrelationId;
        }
        else
        {
            CorrelationContext.CorrelationId = correlationId;
        }

        using (LogContext.PushProperty(LogPropertyNames.CorrelationId, correlationId))
        {
            await next(context);
        }
    }
}
