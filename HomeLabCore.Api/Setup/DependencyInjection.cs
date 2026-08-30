using HomeLabCore.Api.Authorization;
using HomeLabCore.Api.Configuration;
using HomeLabCore.Api.Constants;
using HomeLabCore.Application;
using HomeLabCore.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Serilog;

namespace HomeLabCore.Api.Setup;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplication(this WebApplicationBuilder applicationBuilder)
    {
        Log.Information("Configuring the application services...");

        AddLogging(applicationBuilder);

        var configuration = applicationBuilder.Configuration;

        return applicationBuilder.Services
            .AddInfrastructureServices(configuration)
            .AddApplicationServices(configuration)
            .AddWorkerServices()
            .AddApiServices(configuration);
    }

    private static void AddLogging(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Configuring HomeLabCore.API services...");

        services
            .ConfigureAuthentication(configuration)
            .ConfigureAuthorization();

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }

    private static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyAuthenticationSettings>(
            configuration.GetSection(ApiKeyAuthenticationSettings.SectionName));

        services
            .AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>(AuthenticationSchemes.ApiKey, _ => { });

        return services;
    }

    private static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorization();
    }
}
