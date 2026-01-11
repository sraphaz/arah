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

    public async Task AddPostAsync(CommunityPost post, CancellationToken cancellationToken)
    {
        _dbContext.CommunityPosts.Add(post.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
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
        await _dbContext.SaveChangesAsync(cancellationToken);
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
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCommentAsync(PostComment comment, CancellationToken cancellationToken)
    {
        _dbContext.PostComments.Add(comment.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
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
        await _dbContext.SaveChangesAsync(cancellationToken);
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
}
