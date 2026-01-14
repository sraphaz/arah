namespace Araponga.Application.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Inicia uma transação explícita.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Reverte uma transação ativa.
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Verifica se há uma transação ativa.
    /// </summary>
    Task<bool> HasActiveTransactionAsync(CancellationToken cancellationToken);
}
