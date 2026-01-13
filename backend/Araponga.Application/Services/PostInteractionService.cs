using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Feed;
using Araponga.Domain.Moderation;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for post interactions (likes, comments, shares). Extracted from FeedService.
/// </summary>
public sealed class PostInteractionService
{
    private readonly IFeedRepository _feedRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public PostInteractionService(
        IFeedRepository feedRepository,
        AccessEvaluator accessEvaluator,
        ISanctionRepository sanctionRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _feedRepository = feedRepository;
        _accessEvaluator = accessEvaluator;
        _sanctionRepository = sanctionRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> LikeAsync(
        Guid territoryId,
        Guid postId,
        string actorId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return false;
        }

        if (post.Visibility == PostVisibility.ResidentsOnly)
        {
            if (userId is null)
            {
                return false;
            }

            var membershipRole = await _accessEvaluator.GetRoleAsync(userId.Value, territoryId, cancellationToken);
            if (membershipRole != MembershipRole.Resident)
            {
                return false;
            }
        }

        if (userId is not null)
        {
            var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
                userId.Value,
                territoryId,
                SanctionType.InteractionRestriction,
                DateTime.UtcNow,
                cancellationToken);

            if (interactionRestricted)
            {
                return false;
            }
        }

        await _feedRepository.AddLikeAsync(postId, actorId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<OperationResult> CommentAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        string content,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return OperationResult.Failure("Post not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return OperationResult.Failure("Only residents can comment.");
        }

        var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            userId,
            territoryId,
            SanctionType.InteractionRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (interactionRestricted)
        {
            return OperationResult.Failure("User is restricted from interacting in this territory.");
        }

        var comment = new PostComment(Guid.NewGuid(), postId, userId, content, DateTime.UtcNow);
        await _feedRepository.AddCommentAsync(comment, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("comment.created", userId, territoryId, comment.Id, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> ShareAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return OperationResult.Failure("Post not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return OperationResult.Failure("Only residents can share.");
        }

        var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            userId,
            territoryId,
            SanctionType.InteractionRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (interactionRestricted)
        {
            return OperationResult.Failure("User is restricted from interacting in this territory.");
        }

        await _feedRepository.AddShareAsync(postId, userId, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("post.shared", userId, territoryId, postId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
