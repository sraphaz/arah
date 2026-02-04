using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Postgres;

namespace Araponga.Infrastructure;

/// <summary>
/// Adapta um DbContext (contexto principal) para IUnitOfWork apenas para escopo de transação.
/// CommitAsync é no-op: o commit real do contexto principal é feito via IUnitOfWorkParticipant no CompositeUnitOfWork.
/// Segue Interface Segregation e Single Responsibility: DbContext não implementa IUnitOfWork; apenas este adapter e CompositeUnitOfWork/InMemoryUnitOfWork o fazem.
/// </summary>
public sealed class DbContextTransactionScopeAdapter : IUnitOfWork
{
    private readonly ArapongaDbContext _context;

    public DbContextTransactionScopeAdapter(ArapongaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>No-op: o contexto principal é commitado como participante no CompositeUnitOfWork.</summary>
    public Task CommitAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task BeginTransactionAsync(CancellationToken cancellationToken) => _context.BeginTransactionAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken) => _context.RollbackAsync(cancellationToken);

    public Task<bool> HasActiveTransactionAsync(CancellationToken cancellationToken) => _context.HasActiveTransactionAsync(cancellationToken);
}
