using HomeServerTelegramWorker.Background;
using HomeServerTelegramWorker.Configuration;
using HomeServerTelegramWorker.Telegram.Handlers;
using HomeServerTelegramWorker.Telegram.Handlers.CommandHandlers;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services
    .AddOptions<TelegramSettings>()
    .Bind(configuration.GetSection(TelegramSettings.SectionName))
    .ValidateOnStart()
    .ValidateDataAnnotations();

services.AddSingleton<ITelegramBotClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;

    return new TelegramBotClient(settings.BotToken);
});

services
    .AddScoped<ICommandHandler, HelpCommandHandler>()
    .AddScoped<ICommandHandler, SearchMovieCommandHandler>();

services.AddScoped<IFallbackHandler, FallbackHandler>();

services.AddHostedService<TelegramPollingWorker>();

var host = builder.Build();

await host.RunAsync();
