using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar MembershipCapabilities.
/// Publica eventos para invalidação automática de cache e registra auditoria.
/// </summary>
public sealed class MembershipCapabilityService
{
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public MembershipCapabilityService(
        IMembershipCapabilityRepository capabilityRepository,
        ITerritoryMembershipRepository membershipRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger)
    {
        _capabilityRepository = capabilityRepository;
        _membershipRepository = membershipRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    /// <summary>
    /// Concede uma MembershipCapability e registra auditoria.
    /// </summary>
    public async Task<OperationResult<MembershipCapability>> GrantAsync(
        Guid membershipId,
        MembershipCapabilityType capabilityType,
        Guid? grantedByUserId,
        Guid? grantedByMembershipId,
        string? reason,
        CancellationToken cancellationToken)
    {
        // Verificar se membership existe
        var membership = await _membershipRepository.GetByIdAsync(membershipId, cancellationToken);
        if (membership is null)
        {
            return OperationResult<MembershipCapability>.Failure("Membership not found.");
        }

        // Verificar se já existe capability ativa
        var hasActiveCapability = await _capabilityRepository.HasCapabilityAsync(membershipId, capabilityType, cancellationToken);
        if (hasActiveCapability)
        {
            return OperationResult<MembershipCapability>.Failure("Membership already has this active capability.");
        }

        var grantedAtUtc = DateTime.UtcNow;
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membershipId,
            capabilityType,
            grantedAtUtc,
            grantedByUserId,
            grantedByMembershipId,
            reason);

        await _capabilityRepository.AddAsync(capability, cancellationToken);

        // Registrar auditoria
        await _auditLogger.LogAsync(
            new AuditEntry(
                "membership_capability.granted",
                grantedByUserId ?? Guid.Empty,
                membership.TerritoryId,
                capability.Id,
                grantedAtUtc),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<MembershipCapability>.Success(capability);
    }

    /// <summary>
    /// Revoga uma MembershipCapability, publica evento para invalidação de cache e registra auditoria.
    /// </summary>
    public async Task<OperationResult> RevokeAsync(
        Guid capabilityId,
        Guid revokedByUserId,
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

        // Registrar auditoria
        await _auditLogger.LogAsync(
            new AuditEntry(
                "membership_capability.revoked",
                revokedByUserId,
                membership.TerritoryId,
                capability.Id,
                revokedAtUtc),
            cancellationToken);

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
