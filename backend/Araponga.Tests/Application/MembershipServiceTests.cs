using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MembershipServiceTests
{
    private static readonly Guid TerritoryId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TerritoryId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    // Usar UserId diferente do pré-existente no InMemoryDataStore (aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa)
    private static readonly Guid UserId = Guid.Parse("99999999-9999-9999-9999-999999999999");
    
    // Coordenadas dos territórios (do InMemoryDataStore)
    private const double Territory1Lat = -23.3501;
    private const double Territory1Lng = -44.8912;
    private const double Territory2Lat = -23.3744;
    private const double Territory2Lng = -45.0205;
    
    // Coordenadas próximas (dentro de 5km)
    private const double NearTerritory1Lat = -23.3510;
    private const double NearTerritory1Lng = -44.8920;
    
    // Coordenadas distantes (fora de 5km)
    private const double FarLat = -23.4000;
    private const double FarLng = -44.9500;

    private static MembershipService CreateService(InMemoryDataStore dataStore, InMemorySharedStore sharedStore)
    {
        var repository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        var territoryRepository = new InMemoryTerritoryRepository(sharedStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        return new MembershipService(repository, settingsRepository, territoryRepository, auditLogger, unitOfWork);
    }

    [Fact]
    public async Task EnterAsVisitorAsync_CreatesNewMembership()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var membership = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        Assert.NotNull(membership);
        Assert.Equal(UserId, membership.UserId);
        Assert.Equal(TerritoryId1, membership.TerritoryId);
        Assert.Equal(MembershipRole.Visitor, membership.Role);
        Assert.Equal(ResidencyVerification.None, membership.ResidencyVerification);
    }

    [Fact]
    public async Task EnterAsVisitorAsync_ReturnsExisting_IfAlreadyVisitor()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var first = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);
        var second = await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task BecomeResidentAsync_Succeeds_WhenNoExistingResident()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);

        if (!result.IsSuccess)
        {
            Assert.Fail($"BecomeResidentAsync failed: {result.Error}");
        }
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(MembershipRole.Resident, result.Value!.Role);
        // Não existe mais "fundador" / auto-verificação.
        Assert.Equal(ResidencyVerification.None, result.Value.ResidencyVerification);
    }

    [Fact]
    public async Task BecomeResidentAsync_Fails_WhenHasResidentInAnotherTerritory()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

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
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar primeiro Resident (não auto-verificado)
        var firstUserId = Guid.NewGuid();
        var firstResult = await service.BecomeResidentAsync(firstUserId, TerritoryId1, CancellationToken.None);
        Assert.True(firstResult.IsSuccess);
        Assert.Equal(ResidencyVerification.None, firstResult.Value!.ResidencyVerification);

        // Criar segundo Resident (não verificado)
        var secondUserId = Guid.NewGuid();
        var secondResult = await service.BecomeResidentAsync(secondUserId, TerritoryId1, CancellationToken.None);
        Assert.True(secondResult.IsSuccess);
        Assert.Equal(ResidencyVerification.None, secondResult.Value!.ResidencyVerification);
    }

    [Fact]
    public async Task TransferResidencyAsync_DemotesCurrentResident()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var service = CreateService(dataStore, sharedStore);

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
        Assert.Equal(ResidencyVerification.None, oldMembership.ResidencyVerification);

        // Verificar que território 2 agora é Resident
        var newMembership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId2, CancellationToken.None);
        Assert.NotNull(newMembership);
        Assert.Equal(MembershipRole.Resident, newMembership!.Role);
    }

    [Fact]
    public async Task TransferResidencyAsync_Fails_WhenNoCurrentResident()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.TransferResidencyAsync(UserId, TerritoryId2, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("does not have a Resident", result.Error!);
    }

    [Fact]
    public async Task VerifyResidencyByGeoAsync_UpdatesVerification()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var service = CreateService(dataStore, sharedStore);

        // Criar Resident
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Verificar geo com coordenadas próximas (dentro de 5km)
        var verifyResult = await service.VerifyResidencyByGeoAsync(
            UserId, 
            TerritoryId1, 
            NearTerritory1Lat, 
            NearTerritory1Lng, 
            DateTime.UtcNow, 
            CancellationToken.None);
        Assert.True(verifyResult.IsSuccess);

        // Verificar atualização
        var membership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(ResidencyVerification.GeoVerified, membership!.ResidencyVerification);
        Assert.NotNull(membership.LastGeoVerifiedAtUtc);
    }
    
    [Fact]
    public async Task VerifyResidencyByGeoAsync_Fails_WhenCoordinatesTooFar()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar Resident
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Verificar geo com coordenadas distantes (fora de 5km)
        var verifyResult = await service.VerifyResidencyByGeoAsync(
            UserId, 
            TerritoryId1, 
            FarLat, 
            FarLng, 
            DateTime.UtcNow, 
            CancellationToken.None);
        Assert.True(verifyResult.IsFailure);
        Assert.Contains("too far", verifyResult.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task VerifyResidencyByGeoAsync_Fails_IfNotResident()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar apenas Visitor
        await service.EnterAsVisitorAsync(UserId, TerritoryId1, CancellationToken.None);

        // Tentar verificar geo (deve falhar)
        var result = await service.VerifyResidencyByGeoAsync(
            UserId, 
            TerritoryId1, 
            NearTerritory1Lat, 
            NearTerritory1Lng, 
            DateTime.UtcNow, 
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not a Resident", result.Error!);
    }

    [Fact]
    public async Task VerifyResidencyByDocumentAsync_UpdatesVerification()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var service = CreateService(dataStore, sharedStore);

        // Criar Resident
        var becomeResult = await service.BecomeResidentAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.True(becomeResult.IsSuccess);

        // Verificar documental
        var verifyResult = await service.VerifyResidencyByDocumentAsync(UserId, TerritoryId1, DateTime.UtcNow, CancellationToken.None);
        Assert.True(verifyResult.IsSuccess);

        // Verificar atualização
        var membership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId1, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.True(membership!.IsDocumentVerified());
        Assert.NotNull(membership.LastDocumentVerifiedAtUtc);
    }

    [Fact]
    public async Task ListMyMembershipsAsync_ReturnsAllMemberships()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

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
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

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
