using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
