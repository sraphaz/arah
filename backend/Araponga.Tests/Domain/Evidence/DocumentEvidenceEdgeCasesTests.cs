using Araponga.Domain.Evidence;
using Xunit;

namespace Araponga.Tests.Domain.Evidence;

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
    public void DocumentEvidence_WithEmptyUserId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.Empty,
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("UserId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithNullStorageKey_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                null!,
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("StorageKey", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithEmptyStorageKey_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                "",
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("StorageKey", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithStorageKeyTooLong_ThrowsArgumentException()
    {
        var longKey = new string('a', DocumentEvidence.MaxStorageKeyLength + 1);

        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                longKey,
                ValidContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("StorageKey", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithNullContentType_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                null!,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("ContentType", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithContentTypeTooLong_ThrowsArgumentException()
    {
        var longContentType = new string('a', DocumentEvidence.MaxContentTypeLength + 1);

        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                longContentType,
                1024,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("ContentType", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithZeroSizeBytes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                0,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("SizeBytes", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithNegativeSizeBytes_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                -1,
                ValidSha256,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("SizeBytes", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithNullSha256_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                null!,
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("Sha256", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithInvalidSha256Length_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                "invalid",
                "document.pdf",
                DateTime.UtcNow));

        Assert.Contains("Sha256", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithSha256NormalizedToLower()
    {
        var upperSha = ValidSha256.ToUpperInvariant();

        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            upperSha,
            "document.pdf",
            DateTime.UtcNow);

        Assert.Equal(ValidSha256, evidence.Sha256);
    }

    [Fact]
    public void DocumentEvidence_WithOriginalFileNameTooLong_ThrowsArgumentException()
    {
        var longFileName = new string('a', DocumentEvidence.MaxOriginalFileNameLength + 1);

        var ex = Assert.Throws<ArgumentException>(() =>
            new DocumentEvidence(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                DocumentEvidenceKind.Identity,
                StorageProvider.Local,
                ValidStorageKey,
                ValidContentType,
                1024,
                ValidSha256,
                longFileName,
                DateTime.UtcNow));

        Assert.Contains("OriginalFileName", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DocumentEvidence_WithResidencyKindButNullTerritoryId_ThrowsArgumentException()
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

    [Fact]
    public void DocumentEvidence_WithResidencyKindAndTerritoryId_DoesNotThrow()
    {
        var territoryId = Guid.NewGuid();
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            territoryId,
            DocumentEvidenceKind.Residency,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            ValidSha256,
            "document.pdf",
            DateTime.UtcNow);

        Assert.Equal(territoryId, evidence.TerritoryId);
    }

    [Fact]
    public void DocumentEvidence_WithIdentityKindAndNullTerritoryId_DoesNotThrow()
    {
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            ValidSha256,
            "document.pdf",
            DateTime.UtcNow);

        Assert.Null(evidence.TerritoryId);
    }

    [Fact]
    public void DocumentEvidence_WithStorageKeyTrimmed()
    {
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            "  " + ValidStorageKey + "  ",
            ValidContentType,
            1024,
            ValidSha256,
            "document.pdf",
            DateTime.UtcNow);

        Assert.Equal(ValidStorageKey, evidence.StorageKey);
    }

    [Fact]
    public void DocumentEvidence_WithOriginalFileNameTrimmed()
    {
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            ValidSha256,
            "  document.pdf  ",
            DateTime.UtcNow);

        Assert.Equal("document.pdf", evidence.OriginalFileName);
    }

    [Fact]
    public void DocumentEvidence_WithUnicodeInOriginalFileName_AcceptsUnicode()
    {
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            StorageProvider.Local,
            ValidStorageKey,
            ValidContentType,
            1024,
            ValidSha256,
            "documento_ção.pdf",
            DateTime.UtcNow);

        Assert.Equal("documento_ção.pdf", evidence.OriginalFileName);
    }
}
