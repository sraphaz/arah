using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Connections;

namespace Araponga.Application.Services.Connections;

/// <summary>
/// Serviço de aplicação para configurações de privacidade do módulo de conexões.
/// </summary>
public sealed class ConnectionPrivacyService
{
    private readonly IConnectionPrivacySettingsRepository _privacyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConnectionPrivacyService(
        IConnectionPrivacySettingsRepository privacyRepository,
        IUnitOfWork unitOfWork)
    {
        _privacyRepository = privacyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConnectionPrivacySettings> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await _privacyRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
            return existing;

        var settings = ConnectionPrivacySettings.CreateDefault(userId, DateTime.UtcNow);
        await _privacyRepository.AddAsync(settings, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return settings;
    }

    public async Task<ConnectionPrivacySettings?> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _privacyRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<OperationResult<ConnectionPrivacySettings>> UpdateAsync(
        Guid userId,
        ConnectionRequestPolicy? whoCanAddMe,
        ConnectionVisibility? whoCanSeeMyConnections,
        bool? showConnectionsInProfile,
        CancellationToken cancellationToken)
    {
        var settings = await _privacyRepository.GetByUserIdAsync(userId, cancellationToken);
        if (settings is null)
        {
            settings = ConnectionPrivacySettings.CreateDefault(userId, DateTime.UtcNow);
            await _privacyRepository.AddAsync(settings, cancellationToken);
        }

        var now = DateTime.UtcNow;
        settings.Update(whoCanAddMe, whoCanSeeMyConnections, showConnectionsInProfile, now);
        await _privacyRepository.UpdateAsync(settings, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<ConnectionPrivacySettings>.Success(settings);
    }
}
