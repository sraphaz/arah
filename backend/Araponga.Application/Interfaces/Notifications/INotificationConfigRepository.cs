namespace Araponga.Application.Interfaces.Notifications;

/// <summary>
/// Repositório para configurações de notificações.
/// </summary>
public interface INotificationConfigRepository
{
    /// <summary>
    /// Busca configuração por território ou global (se territoryId for null).
    /// </summary>
    Task<Domain.Notifications.NotificationConfig?> GetByTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken);

    /// <summary>
    /// Busca configuração global.
    /// </summary>
    Task<Domain.Notifications.NotificationConfig?> GetGlobalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Salva ou atualiza configuração.
    /// </summary>
    Task SaveAsync(Domain.Notifications.NotificationConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todas as configurações territoriais.
    /// </summary>
    Task<IReadOnlyList<Domain.Notifications.NotificationConfig>> ListTerritorialAsync(CancellationToken cancellationToken);
}
