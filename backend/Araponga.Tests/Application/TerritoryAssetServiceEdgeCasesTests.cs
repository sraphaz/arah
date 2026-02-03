using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Assets;
using Araponga.Domain.Geo;
using Araponga.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

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
}
