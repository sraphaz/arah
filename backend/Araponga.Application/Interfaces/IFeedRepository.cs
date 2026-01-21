using Araponga.Application.Common;
using Araponga.Domain.Feed;

namespace Araponga.Application.Interfaces;

public interface IFeedRepository
{
    Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommunityPost>> ListByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists posts by territory with pagination.
    /// </summary>
    Task<IReadOnlyList<CommunityPost>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists posts by author with pagination.
    /// </summary>
    Task<IReadOnlyList<CommunityPost>> ListByAuthorPagedAsync(
        Guid authorUserId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts posts by territory.
    /// </summary>
    Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts posts by author.
    /// </summary>
    Task<int> CountByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken);
    Task<CommunityPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken);
    Task AddPostAsync(CommunityPost post, CancellationToken cancellationToken);
    Task UpdatePostAsync(CommunityPost post, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid postId, PostStatus status, CancellationToken cancellationToken);
    Task AddLikeAsync(Guid postId, string actorId, CancellationToken cancellationToken);
    Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken);
    Task AddShareAsync(Guid postId, Guid userId, CancellationToken cancellationToken);
    Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken);
    Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets like and share counts for multiple posts in a single batch operation.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, PostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a post by ID.
    /// </summary>
    Task DeletePostAsync(Guid postId, CancellationToken cancellationToken);
}
