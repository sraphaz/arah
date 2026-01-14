using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Social;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MembershipServiceTests
{
    private static readonly Guid TerritoryId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TerritoryId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    // Usar UserId diferente do pré-existente no InMemoryDataStore (aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa)
    private static readonly Guid UserId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    [Fact]
    public async Task EnterAsVisitorAsync_CreatesNewMembership()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var membership = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        Assert.NotNull(membership);
        Assert.Equal(UserId, membership.UserId);
        Assert.Equal(TerritoryId1, membership.TerritoryId);
        Assert.Equal(MembershipRole.Visitor, membership.Role);
        Assert.Equal(ResidencyVerification.Unverified, membership.ResidencyVerification);
    }

    [Fact]
    public async Task EnterAsVisitorAsync_ReturnsExisting_IfAlreadyVisitor()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var first = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);
        var second = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task BecomeResidentAsync_Succeeds_WhenNoExistingResident()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var result = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);

        if (!result.IsSuccess)
        {
            Assert.True(false, $"BecomeResidentAsync failed: {result.Error}");
        }
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(MembershipRole.Resident, result.Value!.Role);
        Assert.Equal(ResidencyVerification.GeoVerified, result.Value.ResidencyVerification); // Primeiro residente é auto-verificado
    }

    [Fact]
    public async Task BecomeResidentAsync_Fails_WhenHasResidentInAnotherTerritory()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar Resident no território 1
        var firstResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(firstResult.IsSuccess);

        // Tentar criar Resident no território 2 (deve falhar)
        var secondResult = await service.BecomeResidentAsync(UserId, TerritoryId2, CancellationToken.None);

        Assert.True(secondResult.IsFailure);
        Assert.Contains("already has a Resident", secondResult.Error!);
    }

    [Fact]
    public async Task BecomeResidentAsync_SetsUnverified_WhenOtherResidentsExist()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar primeiro Resident (auto-verificado)
        var firstUserId = Guid.NewGuid();
        var firstResult = await service.BecomeResidentAsync(firstUserId, TerritoryId1, CancellationToken.None);
        Assert.True(firstResult.IsSuccess);
        Assert.Equal(ResidencyVerification.GeoVerified, firstResult.Value!.ResidencyVerification);

        // Criar segundo Resident (não verificado)
        var secondUserId = Guid.NewGuid();
        var secondResult = await service.BecomeResidentAsync(secondUserId, TerritoryId1, CancellationToken.None);
        Assert.True(secondResult.IsSuccess);
        Assert.Equal(ResidencyVerification.Unverified, secondResult.Value!.ResidencyVerification);
    }

    [Fact]
    public async Task TransferResidencyAsync_DemotesCurrentResident()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar Resident no território 1
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Transferir para território 2
        var transferResult = await service.TransferResidencyAsync(UserId, TerritoryId2, CancellationToken.None);
        Assert.True(transferResult.IsSuccess);

        // Verificar que território 1 agora é Visitor
        var oldMembership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.NotNull(oldMembership);
        Assert.Equal(MembershipRole.Visitor, oldMembership!.Role);
        Assert.Equal(ResidencyVerification.Unverified, oldMembership.ResidencyVerification);

        // Verificar que território 2 agora é Resident
        var newMembership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId2, CancellationToken.None);
        Assert.NotNull(newMembership);
        Assert.Equal(MembershipRole.Resident, newMembership!.Role);
    }

    [Fact]
    public async Task TransferResidencyAsync_Fails_WhenNoCurrentResident()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var result = await service.TransferResidencyAsync(UserId, TerritoryId2, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("does not have a Resident", result.Error!);
    }

    [Fact]
    public async Task VerifyResidencyByGeoAsync_UpdatesVerification()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar Resident
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Verificar geo
        var verifyResult = await service.VerifyResidencyByGeoAsync(UserId, TerritoryId1, DateTime.UtcNow, CancellationToken.None);
        Assert.True(verifyResult.IsSuccess);

        // Verificar atualização
        var membership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(ResidencyVerification.GeoVerified, membership!.ResidencyVerification);
        Assert.NotNull(membership.LastGeoVerifiedAtUtc);
    }

    [Fact]
    public async Task VerifyResidencyByGeoAsync_Fails_IfNotResident()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar apenas Visitor
        await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        // Tentar verificar geo (deve falhar)
        var result = await service.VerifyResidencyByGeoAsync(UserId, TerritoryId1, DateTime.UtcNow, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not a Resident", result.Error!);
    }

    [Fact]
    public async Task VerifyResidencyByDocumentAsync_UpdatesVerification()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar Resident
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Verificar documental
        var verifyResult = await service.VerifyResidencyByDocumentAsync(UserId, TerritoryId1, DateTime.UtcNow, CancellationToken.None);
        Assert.True(verifyResult.IsSuccess);

        // Verificar atualização
        var membership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(ResidencyVerification.DocumentVerified, membership!.ResidencyVerification);
        Assert.NotNull(membership.LastDocumentVerifiedAtUtc);
    }

    [Fact]
    public async Task ListMyMembershipsAsync_ReturnsAllMemberships()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        // Criar múltiplos Visitors
        await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);
        await service.EnterAsVisitorAsync(UserId, TerritoryId2, CancellationToken.None);

        var memberships = await service.ListMyMembershipsAsync(UserId, CancellationToken.None);

        Assert.Equal(2, memberships.Count);
        Assert.All(memberships, m => Assert.Equal(UserId, m.UserId));
        Assert.All(memberships, m => Assert.Equal(MembershipRole.Visitor, m.Role));
    }

    [Fact]
    public async Task ListMyMembershipsAsync_IncludesMultipleVisitors()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var territory3 = Guid.NewGuid();
        await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);
        await service.EnterAsVisitorAsync(UserId, TerritoryId2, CancellationToken.None);
        await service.EnterAsVisitorAsync(UserId, territory3, CancellationToken.None);

        var memberships = await service.ListMyMembershipsAsync(UserId, CancellationToken.None);

        Assert.Equal(3, memberships.Count);
        var territoryIds = memberships.Select(m => m.TerritoryId).ToHashSet();
        Assert.Contains(TerritoryId1, territoryIds);
        Assert.Contains(TerritoryId2, territoryIds);
        Assert.Contains(territory3, territoryIds);
    }
}
