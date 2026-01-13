using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryFeedRepository : IFeedRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryFeedRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var posts = _dataStore.Posts
            .Where(post => post.TerritoryId == territoryId)
            .ToList();

        return Task.FromResult<IReadOnlyList<CommunityPost>>(posts);
    }

    public Task<IReadOnlyList<CommunityPost>> ListByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken)
    {
        var posts = _dataStore.Posts
            .Where(post => post.AuthorUserId == authorUserId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<CommunityPost>>(posts);
    }

    public Task<CommunityPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var post = _dataStore.Posts.FirstOrDefault(p => p.Id == postId);
        return Task.FromResult(post);
    }

    public Task AddPostAsync(CommunityPost post, CancellationToken cancellationToken)
    {
        _dataStore.Posts.Add(post);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid postId, PostStatus status, CancellationToken cancellationToken)
    {
        var post = _dataStore.Posts.FirstOrDefault(p => p.Id == postId);
        if (post is null)
        {
            return Task.CompletedTask;
        }

        _dataStore.Posts.Remove(post);
        var updated = new CommunityPost(
            post.Id,
            post.TerritoryId,
            post.AuthorUserId,
            post.Title,
            post.Content,
            post.Type,
            post.Visibility,
            status,
            post.MapEntityId,
            post.CreatedAtUtc,
            post.ReferenceType,
            post.ReferenceId);
        _dataStore.Posts.Add(updated);
        return Task.CompletedTask;
    }

    public Task AddLikeAsync(Guid postId, string actorId, CancellationToken cancellationToken)
    {
        if (!_dataStore.PostLikes.TryGetValue(postId, out var likes))
        {
            likes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _dataStore.PostLikes[postId] = likes;
        }

        likes.Add(actorId);
        return Task.CompletedTask;
    }

    public Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken)
    {
        if (!_dataStore.PostComments.TryGetValue(comment.PostId, out var comments))
        {
            comments = new List<PostComment>();
            _dataStore.PostComments[comment.PostId] = comments;
        }

        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task AddShareAsync(Guid postId, Guid userId, CancellationToken cancellationToken)
    {
        if (!_dataStore.PostShares.TryGetValue(postId, out var shares))
        {
            shares = new HashSet<Guid>();
            _dataStore.PostShares[postId] = shares;
        }

        shares.Add(userId);
        return Task.CompletedTask;
    }

    public Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dataStore.PostLikes.TryGetValue(postId, out var likes) ? likes.Count : 0);
    }

    public Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dataStore.PostShares.TryGetValue(postId, out var shares) ? shares.Count : 0);
    }
}
