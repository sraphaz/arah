using Araponga.Domain.Evidence;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class DocumentEvidenceRepositoryTests
{
    [Fact]
    public async Task DocumentEvidenceRepository_AddAndGet()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryDocumentEvidenceRepository(ds);

        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            territoryId: null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            "20260117/abc-file.pdf",
            "application/pdf",
            10,
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "file.pdf",
            DateTime.UtcNow);

        await repo.AddAsync(evidence, CancellationToken.None);

        var found = await repo.GetByIdAsync(evidence.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(evidence.Id, found!.Id);
    }
}

