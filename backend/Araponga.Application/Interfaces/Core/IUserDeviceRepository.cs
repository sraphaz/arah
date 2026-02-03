using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar dispositivos de usuários para notificações push.
/// </summary>
public interface IUserDeviceRepository
{
    /// <summary>
    /// Adiciona um novo dispositivo.
    /// </summary>
    Task AddAsync(UserDevice device, CancellationToken cancellationToken);

    /// <summary>
    /// Busca dispositivo por ID.
    /// </summary>
    Task<UserDevice?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os dispositivos ativos de um usuário.
    /// </summary>
    Task<IReadOnlyList<UserDevice>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Busca dispositivo por token (para evitar duplicatas).
    /// </summary>
    Task<UserDevice?> GetByTokenAsync(string deviceToken, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um dispositivo existente.
    /// </summary>
    Task UpdateAsync(UserDevice device, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um dispositivo.
    /// </summary>
    Task DeleteAsync(Guid deviceId, CancellationToken cancellationToken);
}
