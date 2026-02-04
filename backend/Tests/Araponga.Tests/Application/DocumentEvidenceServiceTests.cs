using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class DocumentEvidenceServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesEvidence_AndWritesAudit()
    {
        var ds = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton(ds);
        services.AddScoped<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<IFileStorage, FakeFileStorage>();
        services.AddScoped<DocumentEvidenceService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<DocumentEvidenceService>();

        await using var content = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        var userId = Guid.NewGuid();

        var result = await svc.CreateAsync(
            userId,
            territoryId: null,
            DocumentEvidenceKind.Identity,
            "doc.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value!.UserId);
        Assert.Equal(DocumentEvidenceKind.Identity, result.Value.Kind);

        Assert.Contains(ds.AuditEntries, e => e.Action == "document_evidence.created" && e.ActorUserId == userId);
    }

    private sealed class FakeFileStorage : IFileStorage
    {
        public StorageProvider Provider => StorageProvider.Local;

        public Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
        {
            // consume stream
            using var ms = new MemoryStream();
            content.CopyTo(ms);
            return Task.FromResult($"fake/{Guid.NewGuid():N}-{fileName}");
        }

        public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
        {
            Stream s = new MemoryStream(new byte[] { 1, 2, 3 });
            return Task.FromResult(s);
        }
    }
}

