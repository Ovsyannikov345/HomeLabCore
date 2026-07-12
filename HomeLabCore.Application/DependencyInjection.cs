using HomeLabCore.Application.Telegram;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers;
using HomeLabCore.Application.Telegram.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLabCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Handlers
        services
            .AddScoped<ICommandHandler, HelpCommandHandler>()
            .AddScoped<ICommandHandler, SearchCommandHandler>();

        services.AddScoped<ICallbackQueryHandler, RequestMediaQueryHandler>();

        services.AddScoped<IFallbackHandler, FallbackHandler>();

        return services;
    }
}
