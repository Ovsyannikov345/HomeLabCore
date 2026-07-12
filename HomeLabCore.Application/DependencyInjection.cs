using HomeLabCore.Application.Telegram;
using HomeLabCore.Application.Telegram.CallbackQueryHandlers;
using HomeLabCore.Application.Telegram.CommandHandlers;
using HomeLabCore.Application.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLabCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IMessageRenderer, MessageRenderer>();

        // Handlers
        services
            .AddScoped<ICommandHandler, HelpCommandHandler>()
            .AddScoped<ICommandHandler, SearchCommandHandler>();

        services
            .AddScoped<ICallbackQueryHandler, RequestMediaQueryHandler>()
            .AddScoped<ICallbackQueryHandler, ChangeSearchPageQueryHandler>();

        services.AddScoped<IFallbackHandler, FallbackHandler>();

        return services;
    }
}
