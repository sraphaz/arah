namespace Araponga.Application.Interfaces;

/// <summary>
/// Unidade de trabalho para persistência. Em Postgres a implementação é um UoW composto que agrega múltiplos contextos:
/// CommitAsync persiste alterações em todos os participantes (ArapongaDbContext, SharedDbContext e DbContexts dos módulos Feed, Events, Map, etc.).
/// Participantes são registrados como <see cref="IUnitOfWorkParticipant"/>; transação explícita (Begin/Rollback/HasActive) delega para o contexto principal.
/// Ver docs/VALIDACAO_INFRAESTRUTURA_INTEGRIDADE.md §3.1.
/// </summary>
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
