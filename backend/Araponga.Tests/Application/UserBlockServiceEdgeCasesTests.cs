using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for UserBlockService,
/// focusing on self-block, user not found, already blocked, unblock then block.
/// </summary>
public sealed class UserBlockServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserBlockRepository _blockRepo;
    private readonly InMemoryUserRepository _userRepo;
    private readonly InMemoryAuditLogger _auditLogger;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly UserBlockService _service;

    public UserBlockServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _blockRepo = new InMemoryUserBlockRepository(_dataStore);
        _userRepo = new InMemoryUserRepository(_dataStore);
        _auditLogger = new InMemoryAuditLogger(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new UserBlockService(
            _blockRepo,
            _userRepo,
            _auditLogger,
            _unitOfWork,
            cacheService: null);
    }

    [Fact]
    public async Task BlockAsync_WhenBlockingSelf_ReturnsError()
    {
        var userId = Guid.NewGuid();
        await AddUserAsync(userId, "Self");

        var (created, error, block) = await _service.BlockAsync(userId, userId, CancellationToken.None);

        Assert.False(created);
        Assert.NotNull(error);
        Assert.Contains("yourself", error, StringComparison.OrdinalIgnoreCase);
        Assert.Null(block);
    }

    [Fact]
    public async Task BlockAsync_WhenBlockedUserNotFound_ReturnsError()
    {
        var blockerId = Guid.NewGuid();
        var blockedId = Guid.NewGuid();
        await AddUserAsync(blockerId, "Blocker");

        var (created, error, block) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);

        Assert.False(created);
        Assert.NotNull(error);
        Assert.Contains("not found", error, StringComparison.OrdinalIgnoreCase);
        Assert.Null(block);
    }

    [Fact]
    public async Task BlockAsync_WhenAlreadyBlocked_ReturnsCreatedFalseAndNullError()
    {
        var blockerId = Guid.NewGuid();
        var blockedId = Guid.NewGuid();
        await AddUserAsync(blockerId, "Blocker");
        await AddUserAsync(blockedId, "Blocked");

        var (created1, _, b1) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.True(created1);
        Assert.NotNull(b1);

        var (created2, error2, b2) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.False(created2);
        Assert.Null(error2);
        Assert.Null(b2);
    }

    [Fact]
    public async Task BlockAsync_WhenValid_CreatesBlock()
    {
        var blockerId = Guid.NewGuid();
        var blockedId = Guid.NewGuid();
        await AddUserAsync(blockerId, "Blocker");
        await AddUserAsync(blockedId, "Blocked");

        var (created, error, block) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);

        Assert.True(created);
        Assert.Null(error);
        Assert.NotNull(block);
        Assert.Equal(blockerId, block.BlockerUserId);
        Assert.Equal(blockedId, block.BlockedUserId);
    }

    [Fact]
    public async Task UnblockAsync_ThenBlockAsync_Works()
    {
        var blockerId = Guid.NewGuid();
        var blockedId = Guid.NewGuid();
        await AddUserAsync(blockerId, "Blocker");
        await AddUserAsync(blockedId, "Blocked");

        var (c1, _, _) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.True(c1);

        await _service.UnblockAsync(blockerId, blockedId, CancellationToken.None);

        var (c2, _, b2) = await _service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.True(c2);
        Assert.NotNull(b2);
    }

    [Fact]
    public async Task UnblockAsync_WhenNotBlocked_DoesNotThrow()
    {
        var blockerId = Guid.NewGuid();
        var blockedId = Guid.NewGuid();
        await AddUserAsync(blockerId, "Blocker");
        await AddUserAsync(blockedId, "Blocked");

        await _service.UnblockAsync(blockerId, blockedId, CancellationToken.None);
    }

    private async Task AddUserAsync(Guid id, string name)
    {
        var user = new User(
            id,
            name,
            "u@x.com",
            "111.111.111-11",
            null,
            null,
            null,
            "google",
            "ext-" + id.ToString("N")[..8],
            DateTime.UtcNow);
        await _userRepo.AddAsync(user, CancellationToken.None);
    }
}
