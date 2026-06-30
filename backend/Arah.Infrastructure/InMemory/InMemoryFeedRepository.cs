using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Domain.Feed;

namespace Arah.Infrastructure.InMemory;

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
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.PostLikes.TryGetValue(postId, out var likes) ? likes.Count : 0;
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.PostShares.TryGetValue(postId, out var shares) ? shares.Count : 0;
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<int> GetCommentCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.PostComments.TryGetValue(postId, out var comments) ? comments.Count : 0;
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<IReadOnlyList<PostComment>> ListCommentsByPostIdPagedAsync(
        Guid postId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        if (!_dataStore.PostComments.TryGetValue(postId, out var comments))
        {
            return Task.FromResult<IReadOnlyList<PostComment>>(Array.Empty<PostComment>());
        }

        var page = comments
            .OrderByDescending(comment => comment.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<PostComment>>(page);
    }

    public Task<int> CountCommentsByPostIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        return GetCommentCountAsync(postId, cancellationToken);
    }

    public Task<IReadOnlyDictionary<Guid, PostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var result = new Dictionary<Guid, PostCounts>();

        foreach (var postId in postIds)
        {
            var likeCountRaw = _dataStore.PostLikes.TryGetValue(postId, out var likes) ? likes.Count : 0;
            var shareCountRaw = _dataStore.PostShares.TryGetValue(postId, out var shares) ? shares.Count : 0;
            var commentCountRaw = _dataStore.PostComments.TryGetValue(postId, out var comments) ? comments.Count : 0;
            var likeCount = likeCountRaw > maxInt32 ? maxInt32 : likeCountRaw;
            var shareCount = shareCountRaw > maxInt32 ? maxInt32 : shareCountRaw;
            var commentCount = commentCountRaw > maxInt32 ? maxInt32 : commentCountRaw;
            result[postId] = new PostCounts(likeCount, shareCount, commentCount);
        }

        return Task.FromResult<IReadOnlyDictionary<Guid, PostCounts>>(result);
    }

    public Task<IReadOnlyList<CommunityPost>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var posts = _dataStore.Posts
            .Where(post => post.TerritoryId == territoryId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<CommunityPost>>(posts);
    }

    public Task<IReadOnlyList<CommunityPost>> ListByAuthorPagedAsync(
        Guid authorUserId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var posts = _dataStore.Posts
            .Where(post => post.AuthorUserId == authorUserId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<CommunityPost>>(posts);
    }

    public Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.Posts.Count(post => post.TerritoryId == territoryId);
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<int> CountByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.Posts.Count(post => post.AuthorUserId == authorUserId);
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task UpdatePostAsync(CommunityPost post, CancellationToken cancellationToken)
    {
        var existingPost = _dataStore.Posts.FirstOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            return Task.CompletedTask;
        }

        _dataStore.Posts.Remove(existingPost);
        _dataStore.Posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeletePostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var post = _dataStore.Posts.FirstOrDefault(p => p.Id == postId);
        if (post is not null)
        {
            _dataStore.Posts.Remove(post);
        }
        return Task.CompletedTask;
    }
}
