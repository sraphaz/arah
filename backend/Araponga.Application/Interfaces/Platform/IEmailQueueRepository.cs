using Araponga.Domain.Email;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para repositório de fila de emails.
/// </summary>
public interface IEmailQueueRepository
{
    /// <summary>
    /// Adiciona um item à fila.
    /// </summary>
    Task<EmailQueueItem> AddAsync(EmailQueueItem item, CancellationToken cancellationToken);

    /// <summary>
    /// Busca itens pendentes para processamento, ordenados por prioridade e data de criação.
    /// </summary>
    Task<IReadOnlyList<EmailQueueItem>> GetPendingItemsAsync(
        int limit,
        CancellationToken cancellationToken);

    /// <summary>
    /// Busca um item por ID.
    /// </summary>
    Task<EmailQueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um item na fila.
    /// </summary>
    Task UpdateAsync(EmailQueueItem item, CancellationToken cancellationToken);

    /// <summary>
    /// Conta itens pendentes.
    /// </summary>
    Task<int> CountPendingAsync(CancellationToken cancellationToken);
}
