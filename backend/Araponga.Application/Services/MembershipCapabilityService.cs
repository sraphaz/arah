using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar MembershipCapabilities.
/// Publica eventos para invalidação automática de cache.
/// </summary>
public sealed class MembershipCapabilityService
{
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public MembershipCapabilityService(
        IMembershipCapabilityRepository capabilityRepository,
        ITerritoryMembershipRepository membershipRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _capabilityRepository = capabilityRepository;
        _membershipRepository = membershipRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Revoga uma MembershipCapability e publica evento para invalidação de cache.
    /// </summary>
    public async Task<OperationResult> RevokeAsync(
        Guid capabilityId,
        CancellationToken cancellationToken)
    {
        var capability = await _capabilityRepository.GetByIdAsync(capabilityId, cancellationToken);
        if (capability is null)
        {
            return OperationResult.Failure("Capability not found.");
        }

        if (!capability.IsActive())
        {
            return OperationResult.Failure("Capability is already revoked.");
        }

        // Buscar membership para obter userId e territoryId
        var membership = await _membershipRepository.GetByIdAsync(capability.MembershipId, cancellationToken);
        if (membership is null)
        {
            return OperationResult.Failure("Membership not found.");
        }

        var revokedAtUtc = DateTime.UtcNow;
        capability.Revoke(revokedAtUtc);

        await _capabilityRepository.UpdateAsync(capability, cancellationToken);

        // Publicar evento para invalidação de cache
        var revokedEvent = new MembershipCapabilityRevokedEvent(
            capability.MembershipId,
            membership.UserId,
            membership.TerritoryId,
            revokedAtUtc);

        await _eventBus.PublishAsync(revokedEvent, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
