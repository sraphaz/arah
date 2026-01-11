using Araponga.Application.Interfaces;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

public sealed class MembershipService
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IAuditLogger _auditLogger;

    public MembershipService(ITerritoryMembershipRepository membershipRepository, IAuditLogger auditLogger)
    {
        _membershipRepository = membershipRepository;
        _auditLogger = auditLogger;
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

        if (existing is not null)
        {
            if (existing.Role == role)
            {
                return existing;
            }

            if (existing.Role == MembershipRole.Resident && role == MembershipRole.Visitor)
            {
                return existing;
            }

            if (existing.Role == MembershipRole.Visitor && role == MembershipRole.Resident)
            {
                existing.UpdateRole(MembershipRole.Resident);
                existing.UpdateVerificationStatus(VerificationStatus.Pending);

                await _membershipRepository.UpdateRoleAndStatusAsync(
                    existing.Id,
                    existing.Role,
                    existing.VerificationStatus,
                    cancellationToken);

                await _auditLogger.LogAsync(
                    new Application.Models.AuditEntry(
                        "membership.upgraded",
                        userId,
                        territoryId,
                        existing.Id,
                        DateTime.UtcNow),
                    cancellationToken);

                return existing;
            }

            return existing;
        }

        var verificationStatus = role == MembershipRole.Resident
            ? VerificationStatus.Pending
            : VerificationStatus.Validated;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            role,
            verificationStatus,
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

        return true;
    }
}
