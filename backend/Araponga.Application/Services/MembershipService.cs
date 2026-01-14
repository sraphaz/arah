using Araponga.Application.Common;
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

    /// <summary>
    /// Entra em um território como Visitor.
    /// Cria ou retorna membership existente.
    /// </summary>
    public async Task<TerritoryMembership> EnterAsVisitorAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var existing = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var visitorMembership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Visitor,
            ResidencyVerification.Unverified,
            null,
            null,
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

    /// <summary>
    /// Solicita tornar-se Resident no território.
    /// Valida regra de exclusividade: máximo 1 Resident por User.
    /// </summary>
    public async Task<Result<TerritoryMembership>> BecomeResidentAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var existing = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        // Se já é Resident no mesmo território, retornar
        if (existing is not null && existing.Role == MembershipRole.Resident)
        {
            return Result<TerritoryMembership>.Success(existing);
        }

        // Validação: 1 Resident por User (verificar se há Resident em outro território)
        var existingResident = await _membershipRepository.GetResidentMembershipAsync(userId, cancellationToken);
        if (existingResident is not null && existingResident.TerritoryId != territoryId)
        {
            return Result<TerritoryMembership>.Failure(
                "User already has a Resident membership in another territory. Transfer residency first.");
        }

        var hasValidatedResident = await _membershipRepository.HasValidatedResidentAsync(territoryId, cancellationToken);
        var residencyVerification = hasValidatedResident
            ? ResidencyVerification.Unverified
            : ResidencyVerification.GeoVerified; // Primeiro residente é auto-verificado

        if (existing is not null)
        {
            // Upgrade de Visitor para Resident
            existing.UpdateRole(MembershipRole.Resident);
            existing.UpdateResidencyVerification(residencyVerification);

            if (!hasValidatedResident)
            {
                existing.UpdateGeoVerification(DateTime.UtcNow);
            }

            // Atualizar tudo de uma vez usando a entidade como fonte da verdade
            await _membershipRepository.UpdateAsync(existing, cancellationToken);

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

            return Result<TerritoryMembership>.Success(existing);
        }

        // Criar novo membership como Resident
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            residencyVerification,
            hasValidatedResident ? null : DateTime.UtcNow,
            null,
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

        return Result<TerritoryMembership>.Success(membership);
    }

    [Obsolete("Use EnterAsVisitorAsync or BecomeResidentAsync instead.")]
    public async Task<TerritoryMembership> DeclareMembershipAsync(
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        CancellationToken cancellationToken)
    {
        if (role == MembershipRole.Visitor)
        {
            return await EnterAsVisitorAsync(userId, territoryId, cancellationToken);
        }

        var result = await BecomeResidentAsync(userId, territoryId, cancellationToken);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        return result.Value!;
    }

    /// <summary>
    /// Transfere residência de um território para outro.
    /// Demove Resident atual e promove no novo território.
    /// </summary>
    public async Task<Result<TerritoryMembership>> TransferResidencyAsync(
        Guid userId,
        Guid toTerritoryId,
        CancellationToken cancellationToken)
    {
        var currentResident = await _membershipRepository.GetResidentMembershipAsync(userId, cancellationToken);
        if (currentResident is null)
        {
            return Result<TerritoryMembership>.Failure("User does not have a Resident membership to transfer.");
        }

        // Demover Resident atual para Visitor
        currentResident.UpdateRole(MembershipRole.Visitor);
        currentResident.UpdateResidencyVerification(ResidencyVerification.Unverified);
        
        await _membershipRepository.UpdateAsync(currentResident, cancellationToken);

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                "membership.residency_transferred_from",
                userId,
                currentResident.TerritoryId,
                currentResident.Id,
                DateTime.UtcNow),
            cancellationToken);

        // Promover no novo território
        var result = await BecomeResidentAsync(userId, toTerritoryId, cancellationToken);
        if (result.IsFailure)
        {
            // Rollback: restaurar Resident anterior
            currentResident.UpdateRole(MembershipRole.Resident);
            currentResident.UpdateResidencyVerification(ResidencyVerification.GeoVerified);
            await _membershipRepository.UpdateAsync(currentResident, cancellationToken);
            return result;
        }

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                "membership.residency_transferred_to",
                userId,
                toTerritoryId,
                result.Value!.Id,
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return result;
    }

    /// <summary>
    /// Verifica residência por geolocalização.
    /// </summary>
    public async Task<OperationResult> VerifyResidencyByGeoAsync(
        Guid userId,
        Guid territoryId,
        DateTime verifiedAtUtc,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (membership is null || membership.Role != MembershipRole.Resident)
        {
            return OperationResult.Failure("Membership not found or user is not a Resident in this territory.");
        }

        membership.UpdateGeoVerification(verifiedAtUtc);
        await _membershipRepository.UpdateGeoVerificationAsync(membership.Id, verifiedAtUtc, cancellationToken);

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                "membership.geo_verified",
                userId,
                territoryId,
                membership.Id,
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Verifica residência por comprovante documental.
    /// </summary>
    public async Task<OperationResult> VerifyResidencyByDocumentAsync(
        Guid userId,
        Guid territoryId,
        DateTime verifiedAtUtc,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (membership is null || membership.Role != MembershipRole.Resident)
        {
            return OperationResult.Failure("Membership not found or user is not a Resident in this territory.");
        }

        membership.UpdateDocumentVerification(verifiedAtUtc);
        await _membershipRepository.UpdateDocumentVerificationAsync(membership.Id, verifiedAtUtc, cancellationToken);

        await _auditLogger.LogAsync(
            new Application.Models.AuditEntry(
                "membership.document_verified",
                userId,
                territoryId,
                membership.Id,
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Lista todos os Memberships do usuário.
    /// </summary>
    public Task<IReadOnlyList<TerritoryMembership>> ListMyMembershipsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _membershipRepository.ListByUserAsync(userId, cancellationToken);
    }

    [Obsolete("Use ResidencyVerification property instead.")]
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

    [Obsolete("Use verification methods instead.")]
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
