using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresFeedRepository : IFeedRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresFeedRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.CommunityPosts
            .AsNoTracking()
            .Where(post => post.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<CommunityPost>> ListByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.CommunityPosts
            .AsNoTracking()
            .Where(post => post.AuthorUserId == authorUserId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<CommunityPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CommunityPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(post => post.Id == postId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddPostAsync(CommunityPost post, CancellationToken cancellationToken)
    {
        _dbContext.CommunityPosts.Add(post.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateStatusAsync(Guid postId, PostStatus status, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CommunityPosts
            .FirstOrDefaultAsync(post => post.Id == postId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = status;
        _dbContext.CommunityPosts.Update(record);
    }

    public async Task AddLikeAsync(Guid postId, string actorId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.PostLikes
            .AsNoTracking()
            .AnyAsync(like => like.PostId == postId && like.ActorId == actorId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.PostLikes.Add(new PostLikeRecord
        {
            PostId = postId,
            ActorId = actorId,
            CreatedAtUtc = DateTime.UtcNow
        });
    }

    public Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken)
    {
        _dbContext.PostComments.Add(comment.ToRecord());
        return Task.CompletedTask;
    }

    public async Task AddShareAsync(Guid postId, Guid userId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.PostShares
            .AsNoTracking()
            .AnyAsync(share => share.PostId == postId && share.UserId == userId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.PostShares.Add(new PostShareRecord
        {
            PostId = postId,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        });
    }

    public async Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostLikes
            .AsNoTracking()
            .CountAsync(like => like.PostId == postId, cancellationToken);
    }

    public async Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostShares
            .AsNoTracking()
            .CountAsync(share => share.PostId == postId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, PostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        if (postIds.Count == 0)
        {
            return new Dictionary<Guid, PostCounts>();
        }

        var likeCounts = await _dbContext.PostLikes
            .AsNoTracking()
            .Where(like => postIds.Contains(like.PostId))
            .GroupBy(like => like.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, cancellationToken);

        var shareCounts = await _dbContext.PostShares
            .AsNoTracking()
            .Where(share => postIds.Contains(share.PostId))
            .GroupBy(share => share.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, cancellationToken);

        var result = new Dictionary<Guid, PostCounts>();
        foreach (var postId in postIds)
        {
            likeCounts.TryGetValue(postId, out var likeCount);
            shareCounts.TryGetValue(postId, out var shareCount);
            result[postId] = new PostCounts(likeCount, shareCount);
        }

        return result;
    }
}
