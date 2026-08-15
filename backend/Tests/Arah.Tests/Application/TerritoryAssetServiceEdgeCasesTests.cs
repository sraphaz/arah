using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Application.Services;
using Arah.Modules.Assets.Domain;
using Arah.Domain.Geo;
using Arah.Modules.Moderation.Domain.Work;
using Arah.Infrastructure.InMemory;
using Arah.Infrastructure.Shared.InMemory;
using Arah.Tests.TestHelpers;
using Xunit;

namespace Arah.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryAssetService,
/// focusing on geo anchor validation, status transitions, and territory validation.
/// </summary>
public class TerritoryAssetServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    [Fact]
    public async Task CreateAsync_WithNullType_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(0, 0)
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            null!,
            "Test Asset",
            null,
            geoAnchors,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Type and name are required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(0, 0)
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "   ",
            null,
            geoAnchors,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Type and name are required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithNullGeoAnchors_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Test Asset",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("At least one geoAnchor is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyGeoAnchors_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Test Asset",
            null,
            new List<TerritoryAssetGeoAnchorInput>(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("At least one geoAnchor is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidGeoAnchor_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(91, 0) // Invalid latitude
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Test Asset",
            null,
            geoAnchors,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid geoAnchors", result.Error ?? "");
    }

    [Fact]
    public async Task UpdateAsync_WithNullType_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(0, 0)
        };

        var result = await assetService.UpdateAsync(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            null!,
            "Test Asset",
            null,
            geoAnchors,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Type and name are required", result.Error ?? "");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentAsset_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(0, 0)
        };

        var result = await assetService.UpdateAsync(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "natural",
            "Test Asset",
            null,
            geoAnchors,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Asset not found", result.Error ?? "");
    }

    [Fact]
    public async Task CurateAsync_WithInvalidOutcome_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var result = await assetService.CurateAsync(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            WorkItemOutcome.None,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid outcome", result.Error ?? "");
    }

    [Fact]
    public async Task ArchiveAsync_WithNonExistentAsset_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var assetService = new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);

        var result = await assetService.ArchiveAsync(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "Reason",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Asset not found", result.Error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithNaturalWaterSubtype_PersistsSubtype()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Rio do Peixe",
            "Curso d'água",
            geoAnchors,
            CancellationToken.None,
            "river");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("natural", result.Value!.Asset.Type);
        Assert.Equal("river", result.Value.Asset.Subtype);
    }

    [Fact]
    public async Task CreateAsync_WithSubtypeOnNonNaturalType_ReturnsFailure()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "cultural",
            "Mirante",
            null,
            geoAnchors,
            CancellationToken.None,
            "river");

        Assert.True(result.IsFailure);
        Assert.Contains("natural", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidNaturalSubtype_ReturnsFailure()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var result = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Lago",
            null,
            geoAnchors,
            CancellationToken.None,
            "lake");

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid natural water subtype", result.Error ?? "");
    }

    [Fact]
    public async Task UpdateAsync_WithNaturalSubtype_UpdatesSubtype()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var created = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Nascente X",
            null,
            geoAnchors,
            CancellationToken.None,
            "spring");
        Assert.True(created.IsSuccess);

        var updated = await assetService.UpdateAsync(
            created.Value!.Asset.Id,
            TestTerritoryId,
            TestUserId,
            "natural",
            "Nascente X",
            null,
            geoAnchors,
            CancellationToken.None,
            "waterfall",
            subtypeSpecified: true);

        Assert.True(updated.IsSuccess);
        Assert.Equal("waterfall", updated.Value!.Asset.Subtype);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtypeOmitted_PreservesExistingSubtype()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var created = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Rio Claro",
            null,
            geoAnchors,
            CancellationToken.None,
            "river");
        Assert.True(created.IsSuccess);

        var updated = await assetService.UpdateAsync(
            created.Value!.Asset.Id,
            TestTerritoryId,
            TestUserId,
            "natural",
            "Rio Claro Atualizado",
            null,
            geoAnchors,
            CancellationToken.None,
            subtype: null,
            subtypeSpecified: false);

        Assert.True(updated.IsSuccess);
        Assert.Equal("river", updated.Value!.Asset.Subtype);
        Assert.Equal("Rio Claro Atualizado", updated.Value.Asset.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtypeExplicitNull_ClearsSubtype()
    {
        var assetService = CreateService();
        var geoAnchors = new List<TerritoryAssetGeoAnchorInput>
        {
            new TerritoryAssetGeoAnchorInput(-23.37, -45.02)
        };

        var created = await assetService.CreateAsync(
            TestTerritoryId,
            TestUserId,
            "natural",
            "Rio Claro",
            null,
            geoAnchors,
            CancellationToken.None,
            "river");
        Assert.True(created.IsSuccess);

        var updated = await assetService.UpdateAsync(
            created.Value!.Asset.Id,
            TestTerritoryId,
            TestUserId,
            "natural",
            "Rio Claro",
            null,
            geoAnchors,
            CancellationToken.None,
            subtype: null,
            subtypeSpecified: true);

        Assert.True(updated.IsSuccess);
        Assert.Null(updated.Value!.Asset.Subtype);
    }

    private static TerritoryAssetService CreateService()
    {
        var dataStore = new InMemoryDataStore();
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var anchorRepository = new InMemoryAssetGeoAnchorRepository(dataStore);
        var validationRepository = new InMemoryAssetValidationRepository(dataStore);
        var sharedStore = new InMemorySharedStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        return new TerritoryAssetService(
            assetRepository,
            anchorRepository,
            validationRepository,
            membershipRepository,
            workItemRepository,
            auditLogger,
            unitOfWork);
    }
}
