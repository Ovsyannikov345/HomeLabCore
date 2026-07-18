using HomeLabCore.Api.Constants;
using HomeLabCore.Shared.Constants;
using Serilog.Context;

namespace HomeLabCore.Api.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var receivedCorrelationIds);

        var correlationId = receivedCorrelationIds.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = context.TraceIdentifier;
        }

        using (LogContext.PushProperty(LogPropertyNames.CorrelationId, correlationId))
        {
            await next(context);
        }
    }
}
