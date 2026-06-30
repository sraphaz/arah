using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Services;
using Arah.Domain.Feed;
using Arah.Domain.Territories;

namespace Arah.Api.Services.Journeys.Backend;

public sealed class InProcessOnboardingJourneyBackend : IOnboardingJourneyBackend
{
    private readonly TerritoryService _territoryService;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly MembershipService _membershipService;
    private readonly FeedService _feedService;
    private readonly IUserRepository _userRepository;

    public InProcessOnboardingJourneyBackend(
        TerritoryService territoryService,
        ActiveTerritoryService activeTerritoryService,
        MembershipService membershipService,
        FeedService feedService,
        IUserRepository userRepository)
    {
        _territoryService = territoryService;
        _activeTerritoryService = activeTerritoryService;
        _membershipService = membershipService;
        _feedService = feedService;
        _userRepository = userRepository;
    }

    public async Task<BackendTerritoryInfo?> GetTerritoryByIdAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var t = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
        return t is null ? null : new BackendTerritoryInfo(t.Id, t.Name, t.Description, t.Status == TerritoryStatus.Active);
    }

    public Task<bool> SetActiveTerritoryAsync(string sessionId, Guid territoryId, CancellationToken cancellationToken = default)
    {
        return _activeTerritoryService.SetActiveAsync(sessionId, territoryId, cancellationToken);
    }

    public async Task<BackendMembershipInfo?> EnterAsVisitorAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken = default)
    {
        var membership = await _membershipService.EnterAsVisitorAsync(userId, territoryId, cancellationToken);
        return new BackendMembershipInfo(membership.Role.ToString().ToUpperInvariant());
    }

    public async Task<BackendUserInfo?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user is null ? null : new BackendUserInfo(user.Id, user.DisplayName, null);
    }

    public async Task<BackendPagedResult<BackendFeedPost>> ListFeedPagedAsync(
        Guid territoryId,
        Guid? userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var paged = await _feedService.ListForTerritoryPagedAsync(
            territoryId, userId, null, null, pagination, false, false, cancellationToken);
        var items = paged.Items.Select(InProcessFeedJourneyBackend.ToBackendPost).ToList();
        return new BackendPagedResult<BackendFeedPost>(
            items, paged.PageNumber, paged.PageSize, paged.TotalCount, paged.TotalPages,
            paged.HasPreviousPage, paged.HasNextPage);
    }

    public async Task<IReadOnlyDictionary<Guid, BackendPostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken = default)
    {
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        return counts.ToDictionary(kv => kv.Key, kv => new BackendPostCounts(kv.Value.LikeCount, kv.Value.ShareCount, kv.Value.CommentCount));
    }

    public async Task<IReadOnlyList<BackendTerritorySuggestion>> GetTerritoriesNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var territories = await _territoryService.NearbyAsync(latitude, longitude, radiusKm, limit, cancellationToken);
        return territories.Select(t => new BackendTerritorySuggestion(t.Id, t.Name, t.Description, t.Latitude, t.Longitude)).ToList();
    }
}
