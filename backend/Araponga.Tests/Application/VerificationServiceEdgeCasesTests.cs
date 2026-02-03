using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Evidence;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for VerificationQueueService,
/// focusing on invalid documents, status transitions, and error handling.
/// </summary>
public class VerificationServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    private static VerificationQueueService CreateService(InMemoryDataStore dataStore, InMemorySharedStore sharedStore)
    {
        var userRepository = new InMemoryUserRepository(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var workItemRepository = new InMemoryWorkItemRepository(dataStore);
        var documentEvidenceRepository = new InMemoryDocumentEvidenceRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        return new VerificationQueueService(
            userRepository,
            membershipRepository,
            workItemRepository,
            documentEvidenceRepository,
            auditLogger,
            unitOfWork);
    }

    [Fact]
    public async Task SubmitIdentityDocumentAsync_WithEmptyEvidenceId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.SubmitIdentityDocumentAsync(
            TestUserId,
            Guid.Empty,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("evidenceId is required", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitIdentityDocumentAsync_WithNonExistentUser_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var evidenceId = Guid.NewGuid();
        var evidence = new DocumentEvidence(
            evidenceId,
            TestUserId,
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            "test/identity.pdf",
            "application/pdf",
            123,
            new string('a', 64),
            "identity.pdf",
            TestDate);
        dataStore.DocumentEvidences.Add(evidence);

        var result = await service.SubmitIdentityDocumentAsync(
            Guid.NewGuid(),
            evidenceId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("User not found", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitIdentityDocumentAsync_WithNonExistentEvidence_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.SubmitIdentityDocumentAsync(
            TestUserId,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Evidence not found", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitIdentityDocumentAsync_WithInvalidEvidenceKind_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var evidenceId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        // Criar evidence com kind Residency ao invés de Identity
        // Residency requer territoryId, então precisamos fornecer um
        var evidence = new DocumentEvidence(
            evidenceId,
            TestUserId,
            territoryId,
            DocumentEvidenceKind.Residency, // Kind incorreto para identity verification
            StorageProvider.Local,
            "test/residency.pdf",
            "application/pdf",
            123,
            new string('a', 64),
            "residency.pdf",
            TestDate);
        dataStore.DocumentEvidences.Add(evidence);

        var result = await service.SubmitIdentityDocumentAsync(
            TestUserId,
            evidenceId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid evidence for identity verification", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitIdentityDocumentAsync_WithEvidenceFromDifferentUser_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var otherUserId = Guid.NewGuid();
        var evidenceId = Guid.NewGuid();
        var evidence = new DocumentEvidence(
            evidenceId,
            otherUserId, // Evidence de outro usuário
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            "test/identity.pdf",
            "application/pdf",
            123,
            new string('a', 64),
            "identity.pdf",
            TestDate);
        dataStore.DocumentEvidences.Add(evidence);

        var result = await service.SubmitIdentityDocumentAsync(
            TestUserId,
            evidenceId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid evidence for identity verification", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitResidencyDocumentAsync_WithEmptyTerritoryId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.SubmitResidencyDocumentAsync(
            TestUserId,
            Guid.Empty,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("territoryId is required", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitResidencyDocumentAsync_WithEmptyEvidenceId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.SubmitResidencyDocumentAsync(
            TestUserId,
            TestTerritoryId,
            Guid.Empty,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("evidenceId is required", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitResidencyDocumentAsync_WithNonExistentMembership_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var evidenceId = Guid.NewGuid();
        var evidence = new DocumentEvidence(
            evidenceId,
            TestUserId,
            TestTerritoryId,
            DocumentEvidenceKind.Residency,
            StorageProvider.Local,
            "test/residency.pdf",
            "application/pdf",
            123,
            new string('a', 64),
            "residency.pdf",
            TestDate);
        dataStore.DocumentEvidences.Add(evidence);

        var result = await service.SubmitResidencyDocumentAsync(
            TestUserId,
            TestTerritoryId,
            evidenceId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Membership not found", result.Error ?? "");
    }

    [Fact]
    public async Task SubmitResidencyDocumentAsync_WithInvalidEvidenceKind_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            TestUserId,
            TestTerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            TestDate);
        sharedStore.Memberships.Add(membership);

        var evidenceId = Guid.NewGuid();
        // Criar evidence com kind Identity ao invés de Residency
        var evidence = new DocumentEvidence(
            evidenceId,
            TestUserId,
            TestTerritoryId,
            DocumentEvidenceKind.Identity, // Kind incorreto
            StorageProvider.Local,
            "test/identity.pdf",
            "application/pdf",
            123,
            new string('a', 64),
            "identity.pdf",
            TestDate);
        dataStore.DocumentEvidences.Add(evidence);

        var result = await service.SubmitResidencyDocumentAsync(
            TestUserId,
            TestTerritoryId,
            evidenceId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid evidence for residency verification", result.Error ?? "");
    }

    [Fact]
    public async Task DecideIdentityAsync_WithNonExistentWorkItem_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.DecideIdentityAsync(
            Guid.NewGuid(),
            TestUserId,
            WorkItemOutcome.Approved,
            "OK",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Work item not found", result.Error ?? "");
    }

    [Fact]
    public async Task DecideResidencyAsync_WithNonExistentWorkItem_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.DecideResidencyAsync(
            Guid.NewGuid(),
            TestUserId,
            WorkItemOutcome.Approved,
            "OK",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Work item not found", result.Error ?? "");
    }
}
