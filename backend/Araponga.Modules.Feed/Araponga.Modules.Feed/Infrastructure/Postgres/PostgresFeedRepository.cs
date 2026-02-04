using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Modules.Feed.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Feed.Infrastructure.Postgres;

public sealed class PostgresFeedRepository : IFeedRepository
{
    private readonly FeedDbContext _dbContext;

    public PostgresFeedRepository(FeedDbContext dbContext)
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
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.PostLikes
            .AsNoTracking()
            .CountAsync(like => like.PostId == postId, cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    public async Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.PostShares
            .AsNoTracking()
            .CountAsync(share => share.PostId == postId, cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    public async Task<IReadOnlyDictionary<Guid, PostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        if (postIds.Count == 0)
        {
            return new Dictionary<Guid, PostCounts>();
        }

        const int maxInt32 = int.MaxValue;

        var likeCounts = await _dbContext.PostLikes
            .AsNoTracking()
            .Where(like => postIds.Contains(like.PostId))
            .GroupBy(like => like.PostId)
            .Select(g => new { PostId = g.Key, Count = (long)g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count > maxInt32 ? maxInt32 : (int)x.Count, cancellationToken);

        var shareCounts = await _dbContext.PostShares
            .AsNoTracking()
            .Where(share => postIds.Contains(share.PostId))
            .GroupBy(share => share.PostId)
            .Select(g => new { PostId = g.Key, Count = (long)g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count > maxInt32 ? maxInt32 : (int)x.Count, cancellationToken);

        var result = new Dictionary<Guid, PostCounts>();
        foreach (var postId in postIds)
        {
            likeCounts.TryGetValue(postId, out var likeCount);
            shareCounts.TryGetValue(postId, out var shareCount);
            var safeLikeCount = likeCount > maxInt32 ? maxInt32 : likeCount;
            var safeShareCount = shareCount > maxInt32 ? maxInt32 : shareCount;
            result[postId] = new PostCounts(safeLikeCount, safeShareCount);
        }

        return result;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.CommunityPosts
            .AsNoTracking()
            .Where(post => post.TerritoryId == territoryId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<CommunityPost>> ListByAuthorPagedAsync(
        Guid authorUserId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.CommunityPosts
            .AsNoTracking()
            .Where(post => post.AuthorUserId == authorUserId)
            .OrderByDescending(post => post.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.CommunityPosts
            .Where(post => post.TerritoryId == territoryId)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    public async Task<int> CountByAuthorAsync(Guid authorUserId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.CommunityPosts
            .Where(post => post.AuthorUserId == authorUserId)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    public async Task UpdatePostAsync(CommunityPost post, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CommunityPosts
            .FirstOrDefaultAsync(p => p.Id == post.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        var updatedRecord = post.ToRecord();
        record.Title = updatedRecord.Title;
        record.Content = updatedRecord.Content;
        record.EditedAtUtc = updatedRecord.EditedAtUtc;
        record.EditCount = updatedRecord.EditCount;
    }

    public async Task DeletePostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CommunityPosts
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (record is not null)
        {
            _dbContext.CommunityPosts.Remove(record);
        }
    }
}
