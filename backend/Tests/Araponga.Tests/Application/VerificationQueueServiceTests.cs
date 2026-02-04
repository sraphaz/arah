using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;
using Araponga.Modules.Moderation.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class VerificationQueueServiceTests
{
    [Fact]
    public async Task SubmitIdentityDocumentAsync_SetsUserPending_AndCreatesWorkItem()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var services = new ServiceCollection();
        services.AddSingleton(dataStore);
        services.AddSingleton(sharedStore);
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<VerificationQueueService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<VerificationQueueService>();
        var userRepo = sp.GetRequiredService<IUserRepository>();

        var user = sharedStore.Users[0];
        Assert.Equal(UserIdentityVerificationStatus.Unverified, user.IdentityVerificationStatus);

        var evidenceId = Guid.NewGuid();
        dataStore.DocumentEvidences.Add(new DocumentEvidence(
            evidenceId,
            user.Id,
            territoryId: null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            storageKey: "test/identity.pdf",
            contentType: "application/pdf",
            sizeBytes: 123,
            sha256: new string('a', 64),
            originalFileName: "identity.pdf",
            createdAtUtc: DateTime.UtcNow));

        var result = await svc.SubmitIdentityDocumentAsync(user.Id, evidenceId, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(WorkItemType.IdentityVerification, result.Value!.Type);
        Assert.Equal(WorkItemStatus.RequiresHumanReview, result.Value.Status);

        var updated = await userRepo.GetByIdAsync(user.Id, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal(UserIdentityVerificationStatus.Pending, updated!.IdentityVerificationStatus);
    }

    [Fact]
    public async Task DecideIdentityAsync_Approve_SetsUserVerified_AndCompletesWorkItem()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var services = new ServiceCollection();
        services.AddSingleton(dataStore);
        services.AddSingleton(sharedStore);
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<VerificationQueueService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<VerificationQueueService>();
        var userRepo = sp.GetRequiredService<IUserRepository>();
        var workRepo = sp.GetRequiredService<IWorkItemRepository>();

        var user = sharedStore.Users[0];

        var evidenceId = Guid.NewGuid();
        dataStore.DocumentEvidences.Add(new DocumentEvidence(
            evidenceId,
            user.Id,
            territoryId: null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            storageKey: "test/identity-2.pdf",
            contentType: "application/pdf",
            sizeBytes: 123,
            sha256: new string('b', 64),
            originalFileName: "identity-2.pdf",
            createdAtUtc: DateTime.UtcNow));

        var submitted = await svc.SubmitIdentityDocumentAsync(user.Id, evidenceId, CancellationToken.None);
        var workItemId = submitted.Value!.Id;

        var adminId = Guid.NewGuid();
        var decide = await svc.DecideIdentityAsync(workItemId, adminId, WorkItemOutcome.Approved, "ok", CancellationToken.None);
        Assert.True(decide.IsSuccess);

        var updatedUser = await userRepo.GetByIdAsync(user.Id, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.Equal(UserIdentityVerificationStatus.Verified, updatedUser!.IdentityVerificationStatus);

        var updatedItem = await workRepo.GetByIdAsync(workItemId, CancellationToken.None);
        Assert.NotNull(updatedItem);
        Assert.Equal(WorkItemStatus.Completed, updatedItem!.Status);
        Assert.Equal(WorkItemOutcome.Approved, updatedItem.Outcome);
    }
}

