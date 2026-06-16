namespace Arah.Api.Services.Journeys.Backend;

public interface IFeedJourneyBackend
{
    Task<bool> IsConnectionsPrioritizeEnabledAsync(Guid territoryId, CancellationToken cancellationToken = default);

    Task<BackendPagedResult<BackendFeedPost>> ListFeedPagedAsync(
        Guid territoryId,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool filterByInterests,
        bool prioritizeConnections,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, BackendPostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, BackendEventSummary>> GetEventSummariesByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> GetMediaUrlsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BackendUserInfo>> GetUsersByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default);

    Task<BackendCreatePostResult> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string content,
        string type,
        string visibility,
        Guid? mapEntityId,
        IReadOnlyList<Guid>? mediaIds,
        IReadOnlyList<string>? tags,
        CancellationToken cancellationToken = default);

    Task<bool> LikeAsync(
        Guid territoryId,
        Guid postId,
        string actorId,
        Guid? userId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error)> CommentAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        string content,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? Error)> ShareAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<BackendFeedPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default);

    Task<BackendPagedResult<BackendPostComment>> ListCommentsPagedAsync(
        Guid territoryId,
        Guid postId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
