using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;

namespace Araponga.Application.Services;

public sealed class UserBlockService
{
    private readonly IUserBlockRepository _blockRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogger _auditLogger;

    public UserBlockService(
        IUserBlockRepository blockRepository,
        IUserRepository userRepository,
        IAuditLogger auditLogger)
    {
        _blockRepository = blockRepository;
        _userRepository = userRepository;
        _auditLogger = auditLogger;
    }

    public async Task<(bool created, string? error, UserBlock? block)> BlockAsync(
        Guid blockerUserId,
        Guid blockedUserId,
        CancellationToken cancellationToken)
    {
        if (blockerUserId == blockedUserId)
        {
            return (false, "Cannot block yourself.", null);
        }

        var blockedUser = await _userRepository.GetByIdAsync(blockedUserId, cancellationToken);
        if (blockedUser is null)
        {
            return (false, "User not found.", null);
        }

        var exists = await _blockRepository.ExistsAsync(blockerUserId, blockedUserId, cancellationToken);
        if (exists)
        {
            return (false, null, null);
        }

        var block = new UserBlock(blockerUserId, blockedUserId, DateTime.UtcNow);
        await _blockRepository.AddAsync(block, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("user.blocked", blockerUserId, Guid.Empty, blockedUserId, DateTime.UtcNow),
            cancellationToken);

        return (true, null, block);
    }
}
