using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Geo;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

public sealed class MembershipService
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private const double GeoVerificationRadiusKm = 5.0; // Raio de 5km para validação de geolocalização

    public MembershipService(
        ITerritoryMembershipRepository membershipRepository,
        ITerritoryRepository territoryRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _membershipRepository = membershipRepository;
        _territoryRepository = territoryRepository;
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

    /// <summary>
    /// Transfere residência de um território para outro.
    /// Demove Resident atual e promove no novo território.
    /// Usa transação explícita para garantir atomicidade.
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

        // Iniciar transação para garantir atomicidade
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
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
                // Rollback automático via transação
                await _unitOfWork.RollbackAsync(cancellationToken);
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

            // Commit da transação (salva mudanças e commita transação)
            await _unitOfWork.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            // Em caso de exceção, fazer rollback
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Verifica residência por geolocalização.
    /// Valida que as coordenadas fornecidas estão dentro do raio permitido do território.
    /// </summary>
    public async Task<OperationResult> VerifyResidencyByGeoAsync(
        Guid userId,
        Guid territoryId,
        double latitude,
        double longitude,
        DateTime verifiedAtUtc,
        CancellationToken cancellationToken)
    {
        // Validar coordenadas
        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return OperationResult.Failure("Invalid latitude or longitude coordinates.");
        }

        // Obter território para validar coordenadas
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return OperationResult.Failure("Territory not found.");
        }

        // Validar que as coordenadas estão dentro do raio permitido do território
        var distance = CalculateDistance(latitude, longitude, territory.Latitude, territory.Longitude);
        if (distance > GeoVerificationRadiusKm)
        {
            return OperationResult.Failure(
                $"Coordinates are too far from territory center. Distance: {distance:F2}km, Maximum allowed: {GeoVerificationRadiusKm}km.");
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (membership is null || membership.Role != MembershipRole.Resident)
        {
            return OperationResult.Failure("Membership not found or user is not a Resident in this territory.");
        }

        membership.UpdateGeoVerification(verifiedAtUtc);
        await _membershipRepository.UpdateAsync(membership, cancellationToken);

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

    /// <summary>
    /// Calcula a distância entre duas coordenadas usando a fórmula de Haversine.
    /// Retorna a distância em quilômetros.
    /// </summary>
    private static double CalculateDistance(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double EarthRadiusKm = 6371.0;
        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);
        var deltaLat = DegreesToRadians(latitude2 - latitude1);
        var deltaLon = DegreesToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

}
