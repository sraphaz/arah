using Araponga.Application.Interfaces;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

public sealed class MembershipService
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public MembershipService(
        ITerritoryMembershipRepository membershipRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _membershipRepository = membershipRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TerritoryMembership> DeclareMembershipAsync(
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        CancellationToken cancellationToken)
    {
        var existing = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (role == MembershipRole.Visitor)
        {
            if (existing is not null)
            {
                return existing;
            }

            var visitorMembership = new TerritoryMembership(
                Guid.NewGuid(),
                userId,
                territoryId,
                role,
                VerificationStatus.Validated,
                DateTime.UtcNow);

            await _membershipRepository.AddAsync(visitorMembership, cancellationToken);

            await _auditLogger.LogAsync(
                new Application.Models.AuditEntry(
                    "membership.declared",
                    userId,
                    territoryId,
                    visitorMembership.Id,
                    DateTime.UtcNow),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return visitorMembership;
        }

        var hasValidatedResident = await _membershipRepository.HasValidatedResidentAsync(territoryId, cancellationToken);
        if (existing is not null)
        {
            if (existing.Role == MembershipRole.Resident)
            {
                return existing;
            }

            var verificationStatus = hasValidatedResident
                ? VerificationStatus.Pending
                : VerificationStatus.Validated;

            existing.UpdateRole(MembershipRole.Resident);
            existing.UpdateVerificationStatus(verificationStatus);

            await _membershipRepository.UpdateRoleAndStatusAsync(
                existing.Id,
                existing.Role,
                existing.VerificationStatus,
                cancellationToken);

            var auditEvent = hasValidatedResident
                ? "membership.upgraded"
                : "membership.founder_validated";

            await _auditLogger.LogAsync(
                new Application.Models.AuditEntry(
                    auditEvent,
                    userId,
                    territoryId,
                    existing.Id,
                    DateTime.UtcNow),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return existing;
        }

        var status = hasValidatedResident ? VerificationStatus.Pending : VerificationStatus.Validated;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            role,
            status,
            DateTime.UtcNow);

        await _membershipRepository.AddAsync(membership, cancellationToken);

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                "membership.declared",
                userId,
                territoryId,
                membership.Id,
                DateTime.UtcNow),
            cancellationToken);

        if (!hasValidatedResident)
        {
            await _auditLogger.LogAsync(
                new Application.Models.AuditEntry(
                    "membership.founder_validated",
                    userId,
                    territoryId,
                    membership.Id,
                    DateTime.UtcNow),
                cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return membership;
    }

    public async Task<VerificationStatus?> GetStatusAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        return membership?.VerificationStatus;
    }

    public async Task<bool> ValidateAsync(
        Guid membershipId,
        Guid curatorId,
        Guid territoryId,
        VerificationStatus status,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByIdAsync(membershipId, cancellationToken);
        if (membership is null || membership.TerritoryId != territoryId)
        {
            return false;
        }

        await _membershipRepository.UpdateStatusAsync(membershipId, status, cancellationToken);

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                $"membership.{status.ToString().ToLowerInvariant()}",
                curatorId,
                territoryId,
                membershipId,
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}
