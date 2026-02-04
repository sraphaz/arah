using Araponga.Domain.Connections;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório de configurações de privacidade de conexões.
/// </summary>
public interface IConnectionPrivacySettingsRepository
{
    Task<ConnectionPrivacySettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ConnectionPrivacySettings> AddAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken);
    Task UpdateAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken);
}
