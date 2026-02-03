using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Metrics;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Users;
using System.Linq;

namespace Araponga.Application.Services;

public sealed class JoinRequestService
{
    private readonly ITerritoryJoinRequestRepository _joinRequestRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipSettingsRepository _settingsRepository;
    private readonly IUserRepository _userRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUnitOfWork _unitOfWork;

    public JoinRequestService(
        ITerritoryJoinRequestRepository joinRequestRepository,
        ITerritoryMembershipRepository membershipRepository,
        IMembershipSettingsRepository settingsRepository,
        IUserRepository userRepository,
        AccessEvaluator accessEvaluator,
        IUnitOfWork unitOfWork)
    {
        _joinRequestRepository = joinRequestRepository;
        _membershipRepository = membershipRepository;
        _settingsRepository = settingsRepository;
        _userRepository = userRepository;
        _accessEvaluator = accessEvaluator;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool created, string? error, TerritoryJoinRequest? request)> CreateAsync(
        Guid requesterUserId,
        Guid territoryId,
        IReadOnlyCollection<Guid> recipientUserIds,
        string? message,
        CancellationToken cancellationToken)
    {
        if (recipientUserIds.Count == 0)
        {
            return (false, "Recipients are required.", null);
        }

        var uniqueRecipients = recipientUserIds.Distinct().ToList();
        if (uniqueRecipients.Count != recipientUserIds.Count)
        {
            return (false, "Recipients must be unique.", null);
        }

        if (uniqueRecipients.Contains(requesterUserId))
        {
            return (false, "Requester cannot be a recipient.", null);
        }

        var requesterMembership = await _membershipRepository.GetByUserAndTerritoryAsync(
            requesterUserId,
            territoryId,
            cancellationToken);

        if (requesterMembership is not null &&
            requesterMembership.Role == MembershipRole.Resident &&
            requesterMembership.HasAnyVerification())
        {
            return (false, "Requester is already a confirmed resident.", null);
        }

        var existing = await _joinRequestRepository.GetPendingByRequesterAsync(
            territoryId,
            requesterUserId,
            cancellationToken);

        if (existing is not null)
        {
            return (false, null, existing);
        }

        var residentUserIds = await _membershipRepository.ListResidentUserIdsAsync(territoryId, cancellationToken);
        var residentLookup = residentUserIds.ToHashSet();

        foreach (var recipientUserId in uniqueRecipients)
        {
            var recipient = await _userRepository.GetByIdAsync(recipientUserId, cancellationToken);
            if (recipient is null)
            {
                return (false, "Recipient not found.", null);
            }

            // Verificar se é resident ou tem capability de Curator no território
            var isRecipientResident = residentLookup.Contains(recipientUserId);
            var isRecipientCurator = await _accessEvaluator.HasCapabilityAsync(
                recipientUserId,
                territoryId,
                MembershipCapabilityType.Curator,
                cancellationToken);

            if (!isRecipientResident && !isRecipientCurator)
            {
                return (false, "Recipient is not a confirmed resident or curator.", null);
            }
        }

        var now = DateTime.UtcNow;
        var request = new TerritoryJoinRequest(
            Guid.NewGuid(),
            territoryId,
            requesterUserId,
            message,
            TerritoryJoinRequestStatus.Pending,
            now,
            null,
            null,
            null);

        var recipients = uniqueRecipients
            .Select(recipientId => new TerritoryJoinRequestRecipient(
                request.Id,
                recipientId,
                now,
                null))
            .ToList();

        await _joinRequestRepository.AddAsync(request, recipients, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Record business metric
        ArapongaMetrics.JoinRequestsCreated.Add(1, new KeyValuePair<string, object?>("territory_id", territoryId));

        return (true, null, request);
    }

    public async Task<IReadOnlyList<IncomingJoinRequest>> ListIncomingAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken)
    {
        var requests = await _joinRequestRepository.ListIncomingAsync(
            recipientUserId,
            TerritoryJoinRequestStatus.Pending,
            cancellationToken);

        var requesterIds = requests.Select(request => request.RequesterUserId).Distinct().ToList();
        var requesterLookup = new Dictionary<Guid, string>();

        foreach (var requesterId in requesterIds)
        {
            var user = await _userRepository.GetByIdAsync(requesterId, cancellationToken);
            if (user is not null)
            {
                requesterLookup[requesterId] = user.DisplayName;
            }
        }

        return requests
            .Select(request => new IncomingJoinRequest(
                request.Id,
                request.TerritoryId,
                request.RequesterUserId,
                requesterLookup.GetValueOrDefault(request.RequesterUserId, string.Empty),
                request.Message,
                request.CreatedAtUtc))
            .ToList();
    }

    public async Task<PagedResult<IncomingJoinRequest>> ListIncomingPagedAsync(
        Guid recipientUserId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var requests = await _joinRequestRepository.ListIncomingAsync(
            recipientUserId,
            TerritoryJoinRequestStatus.Pending,
            cancellationToken);

        var requesterIds = requests.Select(request => request.RequesterUserId).Distinct().ToList();
        var requesterLookup = new Dictionary<Guid, string>();

        foreach (var requesterId in requesterIds)
        {
            var user = await _userRepository.GetByIdAsync(requesterId, cancellationToken);
            if (user is not null)
            {
                requesterLookup[requesterId] = user.DisplayName;
            }
        }

        var incomingRequests = requests
            .Select(request => new IncomingJoinRequest(
                request.Id,
                request.TerritoryId,
                request.RequesterUserId,
                requesterLookup.GetValueOrDefault(request.RequesterUserId, string.Empty),
                request.Message,
                request.CreatedAtUtc))
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList();

        return incomingRequests.ToPagedResult(pagination);
    }

    public async Task<TerritoryJoinRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken)
    {
        return await _joinRequestRepository.GetByIdAsync(requestId, cancellationToken);
    }

    public async Task<JoinRequestDecisionResult> ApproveAsync(
        Guid requestId,
        Guid actorUserId,
        bool isCurator,
        CancellationToken cancellationToken)
    {
        var request = await _joinRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return new JoinRequestDecisionResult(false, false, null, false);
        }

        var isRecipient = await _joinRequestRepository.IsRecipientAsync(requestId, actorUserId, cancellationToken);
        if (!isRecipient && !isCurator)
        {
            return new JoinRequestDecisionResult(true, true, request, false);
        }

        if (request.Status != TerritoryJoinRequestStatus.Pending)
        {
            return new JoinRequestDecisionResult(true, false, request, false);
        }

        var decidedAtUtc = DateTime.UtcNow;
        request.UpdateDecision(TerritoryJoinRequestStatus.Approved, decidedAtUtc, actorUserId);

        await _joinRequestRepository.UpdateStatusAsync(
            requestId,
            TerritoryJoinRequestStatus.Approved,
            decidedAtUtc,
            actorUserId,
            cancellationToken);

        if (isRecipient)
        {
            await _joinRequestRepository.SetRecipientRespondedAtAsync(
                requestId,
                actorUserId,
                decidedAtUtc,
                cancellationToken);
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            request.RequesterUserId,
            request.TerritoryId,
            cancellationToken);

        if (membership is null)
        {
            var newMembership = new TerritoryMembership(
                Guid.NewGuid(),
                request.RequesterUserId,
                request.TerritoryId,
                MembershipRole.Resident,
                ResidencyVerification.None,
                null,
                null,
                decidedAtUtc);

            await _membershipRepository.AddAsync(newMembership, cancellationToken);

            // Criar MembershipSettings automaticamente (similar ao MembershipService.BecomeResidentAsync)
            var existingSettings = await _settingsRepository.GetByMembershipIdAsync(newMembership.Id, cancellationToken);
            if (existingSettings is null)
            {
                var settingsNow = DateTime.UtcNow;
                var newSettings = new MembershipSettings(
                    newMembership.Id,
                    marketplaceOptIn: false,
                    settingsNow,
                    settingsNow);
                await _settingsRepository.AddAsync(newSettings, cancellationToken);
            }
        }
        else if (membership.Role != MembershipRole.Resident ||
                 !membership.HasAnyVerification())
        {
            await _membershipRepository.UpdateRoleAsync(membership.Id, MembershipRole.Resident, cancellationToken);
            // Importante: aprovar JoinRequest promove para Resident, mas não "verifica" residência.
            // ResidencyVerification deve ser feito por geo/documento (ou fluxo admin explícito).
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return new JoinRequestDecisionResult(true, false, request, true);
    }

    public async Task<JoinRequestDecisionResult> RejectAsync(
        Guid requestId,
        Guid actorUserId,
        bool isCurator,
        CancellationToken cancellationToken)
    {
        var request = await _joinRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return new JoinRequestDecisionResult(false, false, null, false);
        }

        var isRecipient = await _joinRequestRepository.IsRecipientAsync(requestId, actorUserId, cancellationToken);
        if (!isRecipient && !isCurator)
        {
            return new JoinRequestDecisionResult(true, true, request, false);
        }

        if (request.Status != TerritoryJoinRequestStatus.Pending)
        {
            return new JoinRequestDecisionResult(true, false, request, false);
        }

        var decidedAtUtc = DateTime.UtcNow;
        request.UpdateDecision(TerritoryJoinRequestStatus.Rejected, decidedAtUtc, actorUserId);

        await _joinRequestRepository.UpdateStatusAsync(
            requestId,
            TerritoryJoinRequestStatus.Rejected,
            decidedAtUtc,
            actorUserId,
            cancellationToken);

        if (isRecipient)
        {
            await _joinRequestRepository.SetRecipientRespondedAtAsync(
                requestId,
                actorUserId,
                decidedAtUtc,
                cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return new JoinRequestDecisionResult(true, false, request, true);
    }

    public async Task<JoinRequestDecisionResult> CancelAsync(
        Guid requestId,
        Guid requesterUserId,
        CancellationToken cancellationToken)
    {
        var request = await _joinRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return new JoinRequestDecisionResult(false, false, null, false);
        }

        if (request.RequesterUserId != requesterUserId)
        {
            return new JoinRequestDecisionResult(true, true, request, false);
        }

        if (request.Status != TerritoryJoinRequestStatus.Pending)
        {
            return new JoinRequestDecisionResult(true, false, request, false);
        }

        var decidedAtUtc = DateTime.UtcNow;
        request.UpdateDecision(TerritoryJoinRequestStatus.Canceled, decidedAtUtc, requesterUserId);

        await _joinRequestRepository.UpdateStatusAsync(
            requestId,
            TerritoryJoinRequestStatus.Canceled,
            decidedAtUtc,
            requesterUserId,
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new JoinRequestDecisionResult(true, false, request, true);
    }
}
