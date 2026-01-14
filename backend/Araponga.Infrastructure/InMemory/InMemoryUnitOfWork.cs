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
    private bool _hasActiveTransaction;

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        // In-memory data store applies changes immediately.
        // No actual commit is needed as there's no transaction boundary.
        // This method exists for interface compatibility and to maintain
        // the same pattern as the Postgres implementation.
        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        // In-memory não suporta transações reais, mas mantém o estado para compatibilidade
        if (_hasActiveTransaction)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        _hasActiveTransaction = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        // In-memory não suporta rollback real, mas reseta o estado
        // Nota: Mudanças já foram aplicadas, então este rollback é apenas simbólico
        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    public Task<bool> HasActiveTransactionAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_hasActiveTransaction);
    }
}
