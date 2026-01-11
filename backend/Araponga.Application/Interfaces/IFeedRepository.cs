using Araponga.Domain.Feed;

namespace Araponga.Application.Interfaces;

public interface IFeedRepository
{
    Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommunityPost>> ListByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken);
    Task<CommunityPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken);
    Task AddPostAsync(CommunityPost post, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid postId, PostStatus status, CancellationToken cancellationToken);
    Task AddLikeAsync(Guid postId, string actorId, CancellationToken cancellationToken);
    Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken);
    Task AddShareAsync(Guid postId, Guid userId, CancellationToken cancellationToken);
    Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken);
    Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken);
}
