using HomeServerTelegramWorker;
using HomeServerTelegramWorker.Background;
using HomeServerTelegramWorker.Configuration;
using HomeServerTelegramWorker.Seerr;
using HomeServerTelegramWorker.Seerr.Handlers;
using HomeServerTelegramWorker.Telegram.Handlers;
using HomeServerTelegramWorker.Telegram.Handlers.CommandHandlers;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

// Configuration
services
    .AddOptions<TelegramSettings>()
    .Bind(configuration.GetSection(TelegramSettings.SectionName))
    .ValidateOnStart()
    .ValidateDataAnnotations();

services.AddOptions<SeerrSettings>()
    .Bind(configuration.GetSection(SeerrSettings.SectionName))
    .ValidateOnStart()
    .ValidateDataAnnotations();

// HTTP Clients
services.AddTransient<SeerrAuthorizationHandler>();

services.AddHttpClient<ISeerrClient, SeerrClient>()
        .AddHttpMessageHandler<SeerrAuthorizationHandler>();

// Telegram
services.AddSingleton<ITelegramBotClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;

    return new TelegramBotClient(settings.BotToken);
});

services
    .AddScoped<ICommandHandler, HelpCommandHandler>()
    .AddScoped<ICommandHandler, SearchCommandHandler>();

services.AddScoped<IFallbackHandler, FallbackHandler>();

// Background Services
services.AddHostedService<TelegramPollingWorker>();

var host = builder.Build();

await host.RunAsync();
