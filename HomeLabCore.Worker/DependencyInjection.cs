using HomeLabCore.Worker.TelegramPolling;
using HomeLabCore.Worker.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HomeLabCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        Log.Information("Configuring HomeLabCore.Worker services...");

        services.AddHostedService<TelegramPollingWorker>();
        services.AddHostedService<WebhookProcessingWorker>();

        return services;
    }
}
