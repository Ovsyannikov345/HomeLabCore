using HomeLabCore.Domain.Entities;

namespace HomeLabCore.Application.Interfaces.Database;

public interface IApplicationDbContext
{
    public IQueryable<T> Query<T>() where T : EntityBase;

    public void Add<T>(T entity) where T: EntityBase;

    public void RemoveRange<T>(IEnumerable<T> entities) where T: EntityBase;

    public Task SaveChanges(CancellationToken ct);
}
