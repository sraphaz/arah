using Araponga.Modules.Moderation.Domain.Evidence;
using Xunit;

namespace Araponga.Tests.Modules.Moderation.Domain.Evidence;

public sealed class DocumentEvidenceEdgeCasesTests
{
    private const string ValidSha256 = "a1b2c3d4e5f6789012345678901234567890abcdef1234567890abcdef123456";
    private const string ValidStorageKey = "evidence/user123/doc.pdf";
    private const string ValidContentType = "application/pdf";

    [Fact]
    public void DocumentEvidence_WithEmptyId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.Empty,
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("ID", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithValidData_CreatesSuccessfully()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var evidence = new DocumentEvidence(
            id,
            userId,
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            ValidSha256,
            "document.pdf",
            createdAt);

        Assert.Equal(id, evidence.Id);
        Assert.Equal(userId, evidence.UserId);
        Assert.Equal(DocumentEvidenceKind.Identity, evidence.Kind);
        Assert.Equal(StorageProvider.Local, evidence.StorageProvider);
        Assert.Equal(1024, evidence.SizeBytes);
    }

    [Fact]
    public void DocumentEvidence_ResidencyKind_RequiresTerritoryId()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Residency,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("TerritoryId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
