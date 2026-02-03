using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure;

/// <summary>
/// Unidade de trabalho que agrega múltiplos participantes (ArapongaDbContext, SharedDbContext, DbContexts dos módulos).
/// CommitAsync persiste alterações em todos os participantes na ordem em que foram injetados.
/// BeginTransactionAsync, RollbackAsync e HasActiveTransactionAsync delegam para o contexto "primário" quando fornecido
/// (ex.: ArapongaDbContext), pois transação distribuída entre vários DbContexts não é implementada.
/// </summary>
public sealed class CompositeUnitOfWork : IUnitOfWork
{
    private readonly IReadOnlyList<IUnitOfWorkParticipant> _participants;
    private readonly IUnitOfWork? _transactionScope;

    public CompositeUnitOfWork(
        IEnumerable<IUnitOfWorkParticipant> participants,
        IUnitOfWork? transactionScope = null)
    {
        _participants = participants?.ToList() ?? throw new ArgumentNullException(nameof(participants));
        _transactionScope = transactionScope;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        foreach (var participant in _participants)
            await participant.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transactionScope is null)
            return Task.CompletedTask;
        return _transactionScope.BeginTransactionAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transactionScope is null)
            return Task.CompletedTask;
        return _transactionScope.RollbackAsync(cancellationToken);
    }

    public Task<bool> HasActiveTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transactionScope is null)
            return Task.FromResult(false);
        return _transactionScope.HasActiveTransactionAsync(cancellationToken);
    }
}
