using Arah.Api.Contracts.Journeys.Feed;

namespace Arah.Api.Services.Journeys;

public interface IFeedJourneyService
{
    Task<TerritoryFeedJourneyResponse?> GetTerritoryFeedAsync(
        Guid territoryId,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool filterByInterests,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken = default);

    Task<CreatePostJourneyResponse?> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        CreatePostJourneyRequest request,
        IReadOnlyList<Guid>? mediaIds,
        CancellationToken cancellationToken = default);

    Task<PostInteractionResponse?> InteractAsync(
        Guid territoryId,
        Guid? userId,
        string sessionIdOrUserId,
        PostInteractionRequest request,
        string? commentContent,
        CancellationToken cancellationToken = default);

    Task<PostCommentsJourneyResponse?> GetPostCommentsAsync(
        Guid territoryId,
        Guid postId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> DeletePostAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
