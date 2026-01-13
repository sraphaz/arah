using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure.InMemory;

/// <summary>
/// In-memory implementation of Unit of Work.
/// Note: In-memory data store does not support true transactions or rollback.
/// Changes are applied immediately. This implementation exists for interface compatibility
/// and to match the pattern used by Postgres implementation.
/// </summary>
public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken)
    {
        // In-memory data store applies changes immediately.
        // No actual commit is needed as there's no transaction boundary.
        // This method exists for interface compatibility and to maintain
        // the same pattern as the Postgres implementation.
        return Task.CompletedTask;
    }
}
