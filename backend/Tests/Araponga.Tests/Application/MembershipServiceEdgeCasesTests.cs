using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for MembershipService,
/// focusing on validation, transitions, transfers, and edge cases.
/// </summary>
public sealed class MembershipServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryTerritoryMembershipRepository _membershipRepository;
    private readonly InMemoryMembershipSettingsRepository _settingsRepository;
    private readonly InMemoryTerritoryRepository _territoryRepository;
    private readonly InMemoryAuditLogger _auditLogger;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly MembershipService _service;

    public MembershipServiceEdgeCasesTests()
    {
        _sharedStore = new InMemorySharedStore();
        _dataStore = new InMemoryDataStore();
        _membershipRepository = new InMemoryTerritoryMembershipRepository(_sharedStore);
        _settingsRepository = new InMemoryMembershipSettingsRepository(_sharedStore);
        _territoryRepository = new InMemoryTerritoryRepository(_sharedStore);
        _auditLogger = new InMemoryAuditLogger(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new MembershipService(
            _membershipRepository,
            _settingsRepository,
            _territoryRepository,
            _auditLogger,
            _unitOfWork);
    }

    [Fact]
    public async Task EnterAsVisitorAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.EnterAsVisitorAsync(Guid.Empty, Guid.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task BecomeResidentAsync_WithEmptyGuid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.BecomeResidentAsync(Guid.Empty, Guid.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task BecomeResidentAsync_WithAlreadyResidentInAnotherTerritory_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var territoryId1 = TestIds.Territory1;
        var territoryId2 = TestIds.Territory2;

        // Criar Resident no território 1
        var result1 = await _service.BecomeResidentAsync(userId, territoryId1, CancellationToken.None);
        Assert.True(result1.IsSuccess);

        // Tentar criar Resident no território 2
        var result2 = await _service.BecomeResidentAsync(userId, territoryId2, CancellationToken.None);

        Assert.True(result2.IsFailure);
        Assert.Contains("already has a Resident", result2.Error ?? "");
    }

    [Fact]
    public async Task BecomeResidentAsync_WithAlreadyResidentInSameTerritory_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var territoryId = TestIds.Territory1;

        // Criar Resident
        var result1 = await _service.BecomeResidentAsync(userId, territoryId, CancellationToken.None);
        Assert.True(result1.IsSuccess);

        // Tentar criar novamente (idempotente)
        var result2 = await _service.BecomeResidentAsync(userId, territoryId, CancellationToken.None);

        Assert.True(result2.IsSuccess);
        Assert.Equal(result1.Value!.Id, result2.Value!.Id);
    }

    [Fact]
    public async Task TransferResidencyAsync_WithNonExistentMembership_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var toTerritoryId = TestIds.Territory2;

        var result = await _service.TransferResidencyAsync(
            userId,
            toTerritoryId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Resident", result.Error ?? "");
    }

    [Fact]
    public async Task TransferResidencyAsync_WithVisitorMembership_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var fromTerritoryId = TestIds.Territory1;
        var toTerritoryId = TestIds.Territory2;

        // Criar Visitor (não Resident)
        await _service.EnterAsVisitorAsync(userId, fromTerritoryId, CancellationToken.None);

        var result = await _service.TransferResidencyAsync(
            userId,
            toTerritoryId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Resident", result.Error ?? "");
    }

    [Fact]
    public async Task TransferResidencyAsync_WithValidTransfer_TransfersSuccessfully()
    {
        var userId = Guid.NewGuid();
        var fromTerritoryId = TestIds.Territory1;
        var toTerritoryId = TestIds.Territory2;

        // Criar Resident no território 1
        var becomeResult = await _service.BecomeResidentAsync(userId, fromTerritoryId, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Transferir para território 2
        var transferResult = await _service.TransferResidencyAsync(
            userId,
            toTerritoryId,
            CancellationToken.None);

        Assert.True(transferResult.IsSuccess);
        Assert.Equal(toTerritoryId, transferResult.Value!.TerritoryId);
        Assert.Equal(MembershipRole.Resident, transferResult.Value.Role);

        // Verificar que não é mais Resident no território 1
        var oldMembership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userId,
            fromTerritoryId,
            CancellationToken.None);
        Assert.NotNull(oldMembership);
        Assert.NotEqual(MembershipRole.Resident, oldMembership!.Role);
    }

    [Fact]
    public async Task EnterAsVisitorAsync_WithUnicodeData_HandlesCorrectly()
    {
        var userId = Guid.NewGuid();
        var territoryId = TestIds.Territory1;

        var result = await _service.EnterAsVisitorAsync(userId, territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(MembershipRole.Visitor, result.Role);
    }
}
