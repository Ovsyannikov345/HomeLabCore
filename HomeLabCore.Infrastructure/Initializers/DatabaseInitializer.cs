using HomeLabCore.Application.Interfaces;
using HomeLabCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeLabCore.Infrastructure.Initializers;

internal sealed class DatabaseInitializer(
    ApplicationDbContext dbContext, 
    ILogger<DatabaseInitializer> logger) 
    : IApplicationInitializer
{
    public int Order => -100;

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Applying pending database migrations...");

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Database migration failed during startup.");

            throw;
        }
    }
}
