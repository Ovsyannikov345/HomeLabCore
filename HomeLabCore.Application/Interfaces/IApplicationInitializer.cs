namespace HomeLabCore.Application.Interfaces;

public interface IApplicationInitializer
{
    public int Order { get; }

    public Task InitializeAsync(CancellationToken cancellationToken);
}
