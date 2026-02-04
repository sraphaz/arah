using Araponga.Application.Interfaces.Users;
using Araponga.Application.Services.Users;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for UserMediaPreferencesService,
/// focusing on null/empty inputs, user ID mismatch, and default preferences.
/// </summary>
public sealed class UserMediaPreferencesServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserMediaPreferencesRepository _preferencesRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly UserMediaPreferencesService _service;

    public UserMediaPreferencesServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _preferencesRepository = new InMemoryUserMediaPreferencesRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new UserMediaPreferencesService(_preferencesRepository, _unitOfWork);
    }

    [Fact]
    public async Task GetPreferencesAsync_WithEmptyGuid_ReturnsDefaultPreferences()
    {
        var result = await _service.GetPreferencesAsync(Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.UserId);
        Assert.True(result.ShowImages); // Default
        Assert.True(result.ShowVideos); // Default
        Assert.True(result.ShowAudio); // Default
        Assert.False(result.AutoPlayVideos); // Default
        Assert.False(result.AutoPlayAudio); // Default
    }

    [Fact]
    public async Task GetPreferencesAsync_WithNonExistentUser_ReturnsDefaultPreferences()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetPreferencesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.ShowImages); // Default
    }

    [Fact]
    public async Task GetPreferencesAsync_WithExistingPreferences_ReturnsSavedPreferences()
    {
        var userId = Guid.NewGuid();
        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = false,
            ShowVideos = true,
            ShowAudio = false,
            AutoPlayVideos = true,
            AutoPlayAudio = false,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _preferencesRepository.SaveAsync(preferences, CancellationToken.None);

        var result = await _service.GetPreferencesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.False(result.ShowImages);
        Assert.True(result.ShowVideos);
        Assert.False(result.ShowAudio);
        Assert.True(result.AutoPlayVideos);
        Assert.False(result.AutoPlayAudio);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithNullPreferences_ThrowsNullReferenceException()
    {
        var userId = Guid.NewGuid();
        // O serviço não valida null explicitamente, então lança NullReferenceException
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _service.UpdatePreferencesAsync(userId, null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithUserIdMismatch_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var preferences = new UserMediaPreferences
        {
            UserId = differentUserId,
            ShowImages = true,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpdatePreferencesAsync(userId, preferences, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithMatchingUserId_UpdatesPreferences()
    {
        var userId = Guid.NewGuid();
        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = false,
            ShowVideos = false,
            ShowAudio = false,
            AutoPlayVideos = true,
            AutoPlayAudio = true,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await _service.UpdatePreferencesAsync(userId, preferences, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.False(result.ShowImages);
        Assert.False(result.ShowVideos);
        Assert.False(result.ShowAudio);
        Assert.True(result.AutoPlayVideos);
        Assert.True(result.AutoPlayAudio);
        // Verifica que o timestamp foi atualizado (pode ser igual ou maior devido à velocidade de execução)
        Assert.True(result.UpdatedAtUtc >= preferences.UpdatedAtUtc);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithAllPreferencesDisabled_HandlesCorrectly()
    {
        var userId = Guid.NewGuid();
        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = false,
            ShowVideos = false,
            ShowAudio = false,
            AutoPlayVideos = false,
            AutoPlayAudio = false,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await _service.UpdatePreferencesAsync(userId, preferences, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.ShowImages);
        Assert.False(result.ShowVideos);
        Assert.False(result.ShowAudio);
        Assert.False(result.AutoPlayVideos);
        Assert.False(result.AutoPlayAudio);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithAllPreferencesEnabled_HandlesCorrectly()
    {
        var userId = Guid.NewGuid();
        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = true,
            ShowVideos = true,
            ShowAudio = true,
            AutoPlayVideos = true,
            AutoPlayAudio = true,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await _service.UpdatePreferencesAsync(userId, preferences, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.ShowImages);
        Assert.True(result.ShowVideos);
        Assert.True(result.ShowAudio);
        Assert.True(result.AutoPlayVideos);
        Assert.True(result.AutoPlayAudio);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_UpdatesTimestamp()
    {
        var userId = Guid.NewGuid();
        var originalTime = DateTime.UtcNow.AddMinutes(-10);
        var preferences = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = true,
            UpdatedAtUtc = originalTime
        };

        await Task.Delay(100); // Pequeno delay para garantir diferença de timestamp
        var result = await _service.UpdatePreferencesAsync(userId, preferences, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.UpdatedAtUtc > originalTime);
    }

    [Fact]
    public async Task GetPreferencesAsync_AfterUpdate_ReturnsUpdatedPreferences()
    {
        var userId = Guid.NewGuid();
        
        // Obter preferências padrão
        var defaultPrefs = await _service.GetPreferencesAsync(userId, CancellationToken.None);
        Assert.True(defaultPrefs.ShowImages);

        // Atualizar preferências
        var updatedPrefs = new UserMediaPreferences
        {
            UserId = userId,
            ShowImages = false,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _service.UpdatePreferencesAsync(userId, updatedPrefs, CancellationToken.None);

        // Obter novamente
        var result = await _service.GetPreferencesAsync(userId, CancellationToken.None);
        Assert.False(result.ShowImages);
    }

    [Fact]
    public async Task UpdatePreferencesAsync_WithEmptyGuid_HandlesGracefully()
    {
        var preferences = new UserMediaPreferences
        {
            UserId = Guid.Empty,
            ShowImages = false,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var result = await _service.UpdatePreferencesAsync(Guid.Empty, preferences, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.UserId);
        Assert.False(result.ShowImages);
    }
}
