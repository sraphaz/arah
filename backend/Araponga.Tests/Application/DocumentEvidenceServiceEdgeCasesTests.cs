using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Evidence;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for DocumentEvidenceService (validation failures, empty file, too large, unsupported type).
/// </summary>
public sealed class DocumentEvidenceServiceEdgeCasesTests
{
    private static DocumentEvidenceService CreateService(InMemoryDataStore ds)
    {
        var services = new ServiceCollection();
        services.AddSingleton(ds);
        services.AddScoped<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<IFileStorage, FakeFileStorage>();
        services.AddScoped<DocumentEvidenceService>();
        return services.BuildServiceProvider().GetRequiredService<DocumentEvidenceService>();
    }

    [Fact]
    public async Task CreateAsync_WithEmptyUserId_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        await using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await svc.CreateAsync(
            Guid.Empty,
            null,
            DocumentEvidenceKind.Identity,
            "x.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("userId", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithResidencyKindAndNoTerritoryId_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        await using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await svc.CreateAsync(
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Residency,
            "x.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("territoryId", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithUnsupportedContentType_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        await using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await svc.CreateAsync(
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            "x.bin",
            "application/octet-stream",
            content,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("content type", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyFile_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        await using var content = new MemoryStream(Array.Empty<byte>());

        var result = await svc.CreateAsync(
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            "x.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("empty", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithFileTooLarge_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var overLimit = (5 * 1024 * 1024) + 1;
        var bytes = new byte[overLimit];
        new Random(42).NextBytes(bytes);
        await using var content = new MemoryStream(bytes);

        var result = await svc.CreateAsync(
            Guid.NewGuid(),
            null,
            DocumentEvidenceKind.Identity,
            "big.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("large", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithResidencyAndTerritoryId_Succeeds()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var territoryId = Guid.NewGuid();
        await using var content = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        var result = await svc.CreateAsync(
            Guid.NewGuid(),
            territoryId,
            DocumentEvidenceKind.Residency,
            "r.pdf",
            "application/pdf",
            content,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(territoryId, result.Value!.TerritoryId);
    }

    private sealed class FakeFileStorage : IFileStorage
    {
        public StorageProvider Provider => StorageProvider.Local;

        public Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();
            content.CopyTo(ms);
            return Task.FromResult($"fake/{Guid.NewGuid():N}-{fileName}");
        }

        public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(new MemoryStream(new byte[] { 1, 2, 3 }));
        }
    }
}
