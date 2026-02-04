using Araponga.Domain.Notifications;

namespace Araponga.Application.Interfaces.Notifications;

/// <summary>
/// Repositório para configurações de notificações.
/// </summary>
public interface INotificationConfigRepository
{
    /// <summary>
    /// Busca configuração por território ou global (se territoryId for null).
    /// </summary>
    Task<NotificationConfig?> GetByTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken);

    /// <summary>
    /// Busca configuração global.
    /// </summary>
    Task<NotificationConfig?> GetGlobalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Salva ou atualiza configuração.
    /// </summary>
    Task SaveAsync(NotificationConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todas as configurações territoriais.
    /// </summary>
    Task<IReadOnlyList<NotificationConfig>> ListTerritorialAsync(CancellationToken cancellationToken);
}
