using Araponga.Application.Interfaces;
using Araponga.Application.Services.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.InMemory;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for MediaStorageConfigService.
/// </summary>
public sealed class MediaStorageConfigServiceEdgeCasesTests
{
    private static MediaStorageSettings S3Settings(string bucket, string region = "region", string accessKeyId = "key") =>
        new() { S3 = new S3StorageSettings(bucket, region, accessKeyId, null) };

    private static MediaStorageSettings LocalSettings(string basePath = "wwwroot/media") =>
        new() { Local = new LocalStorageSettings(basePath) };

    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryMediaStorageConfigRepository _repository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly Mock<IAuditLogger> _auditLoggerMock;
    private readonly MediaStorageConfigService _service;

    public MediaStorageConfigServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _repository = new InMemoryMediaStorageConfigRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _auditLoggerMock = new Mock<IAuditLogger>();
        _service = new MediaStorageConfigService(_repository, _unitOfWork, _auditLoggerMock.Object);
    }

    [Fact]
    public async Task GetActiveConfigAsync_WhenNoActiveConfig_ReturnsNull()
    {
        // Act
        var result = await _service.GetActiveConfigAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateConfigAsync_WithValidData_CreatesInactiveConfig()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var settings = S3Settings("bucket-name");

        // Act
        var config = await _service.CreateConfigAsync(
            MediaStorageProvider.S3,
            settings,
            userId,
            "Test config",
            CancellationToken.None);

        // Assert
        Assert.NotNull(config);
        Assert.False(config.IsActive); // Nova configuração é inativa por padrão
        Assert.Equal(MediaStorageProvider.S3, config.Provider);
        Assert.Equal("Test config", config.Description);
    }

    [Fact]
    public async Task ActivateConfigAsync_WhenConfigExists_ActivatesAndDeactivatesOthers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var settings1 = S3Settings("bucket1");
        var settings2 = LocalSettings("base-path-2");

        var config1 = await _service.CreateConfigAsync(
            MediaStorageProvider.S3,
            settings1,
            userId,
            "Config 1",
            CancellationToken.None);

        var config2 = await _service.CreateConfigAsync(
            MediaStorageProvider.Local,
            settings2,
            userId,
            "Config 2",
            CancellationToken.None);

        // Ativar config1 primeiro
        await _service.ActivateConfigAsync(config1.Id, userId, CancellationToken.None);

        // Act - Ativar config2 (deve desativar config1)
        var activated = await _service.ActivateConfigAsync(config2.Id, userId, CancellationToken.None);

        // Assert
        Assert.True(activated.IsActive);
        var allConfigs = await _service.ListAllAsync(CancellationToken.None);
        var activeConfigs = allConfigs.Where(c => c.IsActive).ToList();
        Assert.Single(activeConfigs);
        Assert.Equal(config2.Id, activeConfigs[0].Id);
    }

    [Fact]
    public async Task UpdateConfigAsync_WhenConfigExists_UpdatesSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var settings = S3Settings("bucket");
        var config = await _service.CreateConfigAsync(
            MediaStorageProvider.S3,
            settings,
            userId,
            "Original",
            CancellationToken.None);

        var newSettings = S3Settings("new-bucket", "new-region", "new-key");

        // Act
        var updated = await _service.UpdateConfigAsync(
            config.Id,
            newSettings,
            userId,
            "Updated",
            CancellationToken.None);

        // Assert
        Assert.Equal("Updated", updated.Description);
        Assert.NotNull(updated.Settings.S3);
        Assert.Equal("new-bucket", updated.Settings.S3.BucketName);
    }

    [Fact]
    public async Task UpdateConfigAsync_WhenConfigNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentId = Guid.NewGuid();
        var settings = S3Settings("bucket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateConfigAsync(
                nonExistentId,
                settings,
                userId,
                null,
                CancellationToken.None));
    }

    [Fact]
    public async Task ActivateConfigAsync_WhenConfigNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.ActivateConfigAsync(nonExistentId, userId, CancellationToken.None));
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllConfigs()
    {
        // Arrange
        var userId = Guid.NewGuid();

        await _service.CreateConfigAsync(MediaStorageProvider.S3, S3Settings("bucket"), userId, "Config 1", CancellationToken.None);
        await _service.CreateConfigAsync(MediaStorageProvider.Local, LocalSettings(), userId, "Config 2", CancellationToken.None);

        // Act
        var allConfigs = await _service.ListAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, allConfigs.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenConfigExists_ReturnsConfig()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var created = await _service.CreateConfigAsync(
            MediaStorageProvider.S3,
            S3Settings("bucket"),
            userId,
            "Test",
            CancellationToken.None);

        // Act
        var retrieved = await _service.GetByIdAsync(created.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("Test", retrieved.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenConfigNotExists_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
