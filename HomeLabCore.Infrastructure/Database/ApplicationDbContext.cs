using HomeLabCore.Application.Interfaces.Database;
using HomeLabCore.Domain.Entities;
using HomeLabCore.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;

namespace HomeLabCore.Infrastructure.Database;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<MediaSearchSnapshot> MediaSearchSnapshots => Set<MediaSearchSnapshot>();

    public DbSet<MediaSubscription> MediaSubscriptions => Set<MediaSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public IQueryable<T> Query<T>() where T : EntityBase
    {
        return Set<T>().AsQueryable();
    }

    void IApplicationDbContext.Add<T>(T entity)
    {
        Set<T>().Add(entity);
    }

    public void RemoveRange<T>(IEnumerable<T> entities) where T : EntityBase
    {
        Set<T>().RemoveRange(entities);
    }

    public Task SaveChanges(CancellationToken ct)
    {
        return SaveChangesAsync(ct);
    }
}
