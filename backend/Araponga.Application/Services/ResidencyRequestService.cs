using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Orquestra o pedido de residência: o usuário permanece Visitor e cria um JoinRequest.
/// Regras:
/// - Se o usuário conhece alguém, pode direcionar o pedido (recipientUserIds).
/// - Se não informar destinatários, o pedido vai para Curators do território.
/// - Se não houver Curator, faz fallback para SystemAdmins (perm. global).
/// </summary>
public sealed class ResidencyRequestService
{
    private readonly JoinRequestService _joinRequestService;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ISystemPermissionRepository _systemPermissionRepository;
    private readonly ITerritoryJoinRequestRepository _joinRequestRepository;
    private readonly IMemoryCache _cache;

    private const int MaxInviteRecipients = 3;
    private const int MaxCreatedRequestsPerTerritoryPerDay = 3;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromHours(24);

    public ResidencyRequestService(
        JoinRequestService joinRequestService,
        ITerritoryMembershipRepository membershipRepository,
        IMembershipCapabilityRepository capabilityRepository,
        ISystemPermissionRepository systemPermissionRepository,
        ITerritoryJoinRequestRepository joinRequestRepository,
        IMemoryCache cache)
    {
        _joinRequestService = joinRequestService;
        _membershipRepository = membershipRepository;
        _capabilityRepository = capabilityRepository;
        _systemPermissionRepository = systemPermissionRepository;
        _joinRequestRepository = joinRequestRepository;
        _cache = cache;
    }

    public async Task<Result<ResidencyRequestResult>> RequestAsync(
        Guid requesterUserId,
        Guid territoryId,
        IReadOnlyCollection<Guid>? recipientUserIds,
        string? message,
        CancellationToken cancellationToken)
    {
        if (recipientUserIds is not null && recipientUserIds.Count > MaxInviteRecipients)
        {
            return Result<ResidencyRequestResult>.Failure(
                $"Too many recipients. Maximum allowed is {MaxInviteRecipients}.");
        }

        // Regra: 1 Resident por User - se já for Resident em outro território, precisa transferir.
        var existingResident = await _membershipRepository.GetResidentMembershipAsync(requesterUserId, cancellationToken);
        if (existingResident is not null && existingResident.TerritoryId != territoryId)
        {
            return Result<ResidencyRequestResult>.Failure(
                "User already has a Resident membership in another territory. Transfer residency first.");
        }

        // Se já é Resident no território, não faz sentido pedir.
        var existing = await _membershipRepository.GetByUserAndTerritoryAsync(requesterUserId, territoryId, cancellationToken);
        if (existing is not null && existing.Role == MembershipRole.Resident)
        {
            return Result<ResidencyRequestResult>.Failure("User is already a Resident in this territory.");
        }

        // Idempotência: se já existe um pedido pendente, não conta para rate-limit.
        var pending = await _joinRequestRepository.GetPendingByRequesterAsync(territoryId, requesterUserId, cancellationToken);
        if (pending is not null)
        {
            return Result<ResidencyRequestResult>.Success(new ResidencyRequestResult(false, pending));
        }

        // Rate limit: contar apenas criações novas (cancel/recreate entra aqui).
        if (IsRateLimited(requesterUserId, territoryId))
        {
            return Result<ResidencyRequestResult>.Failure(
                "Too many residency requests for this territory. Try again later.");
        }

        var recipients = await BuildRecipientsAsync(requesterUserId, territoryId, recipientUserIds, cancellationToken);
        if (recipients.Count == 0)
        {
            return Result<ResidencyRequestResult>.Failure("No approvers available for this territory.");
        }

        var (created, error, joinRequest) = await _joinRequestService.CreateAsync(
            requesterUserId,
            territoryId,
            recipients,
            message,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(error))
        {
            return Result<ResidencyRequestResult>.Failure(error);
        }

        if (joinRequest is null)
        {
            return Result<ResidencyRequestResult>.Failure("Unable to create join request.");
        }

        if (created)
        {
            RegisterCreatedRequest(requesterUserId, territoryId);
        }

        return Result<ResidencyRequestResult>.Success(new ResidencyRequestResult(created, joinRequest));
    }

    private bool IsRateLimited(Guid requesterUserId, Guid territoryId)
    {
        var key = GetRateLimitKey(requesterUserId, territoryId);
        if (_cache.TryGetValue<RateLimitState?>(key, out var state) && state is not null)
        {
            return state.CreatedCount >= MaxCreatedRequestsPerTerritoryPerDay;
        }

        return false;
    }

    private void RegisterCreatedRequest(Guid requesterUserId, Guid territoryId)
    {
        var key = GetRateLimitKey(requesterUserId, territoryId);
        if (_cache.TryGetValue<RateLimitState?>(key, out var existing) && existing is not null)
        {
            existing.CreatedCount += 1;
            _cache.Set(key, existing, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = RateLimitWindow
            });
            return;
        }

        _cache.Set(key, new RateLimitState { CreatedCount = 1 }, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = RateLimitWindow
        });
    }

    private static string GetRateLimitKey(Guid requesterUserId, Guid territoryId)
        => $"residency_request:rate:{requesterUserId}:{territoryId}";

    private async Task<IReadOnlyList<Guid>> BuildRecipientsAsync(
        Guid requesterUserId,
        Guid territoryId,
        IReadOnlyCollection<Guid>? recipientUserIds,
        CancellationToken cancellationToken)
    {
        // Caso A: pedido direcionado (convite)
        if (recipientUserIds is not null && recipientUserIds.Count > 0)
        {
            return recipientUserIds
                .Where(id => id != requesterUserId)
                .Distinct()
                .ToList();
        }

        // Caso B: pedido "aberto" (sem destinatários) -> Curators do território
        var curatorMembershipIds = await _capabilityRepository.ListMembershipIdsWithCapabilityAsync(
            MembershipCapabilityType.Curator,
            territoryId,
            cancellationToken);

        var curatorUserIds = new List<Guid>(curatorMembershipIds.Count);
        foreach (var membershipId in curatorMembershipIds)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId, cancellationToken);
            if (membership is not null)
            {
                curatorUserIds.Add(membership.UserId);
            }
        }

        var recipients = curatorUserIds
            .Where(id => id != requesterUserId)
            .Distinct()
            .ToList();

        // Fallback: SystemAdmins (governança global)
        if (recipients.Count == 0)
        {
            var adminUserIds = await _systemPermissionRepository.ListUserIdsWithPermissionAsync(
                SystemPermissionType.SystemAdmin,
                cancellationToken);

            recipients = adminUserIds
                .Where(id => id != requesterUserId)
                .Distinct()
                .ToList();
        }

        return recipients;
    }

    private sealed class RateLimitState
    {
        public int CreatedCount { get; set; }
    }
}

