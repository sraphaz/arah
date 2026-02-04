namespace Araponga.Application.Interfaces;

/// <summary>
/// Participante de uma unidade de trabalho composta. Permite que múltiplos contextos de persistência
/// (ArapongaDbContext, SharedDbContext, DbContexts dos módulos) sejam commitados em uma única chamada
/// a IUnitOfWork.CommitAsync quando a implementação de IUnitOfWork for um CompositeUnitOfWork.
/// </summary>
public interface IUnitOfWorkParticipant
{
    Task CommitAsync(CancellationToken cancellationToken);
}
