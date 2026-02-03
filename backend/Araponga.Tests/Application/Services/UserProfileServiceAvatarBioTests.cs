using Araponga.Application.Exceptions;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Media;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application.Services;

/// <summary>
/// Testes unitários para funcionalidades de avatar e bio do UserProfileService.
/// </summary>
public sealed class UserProfileServiceAvatarBioTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryUserRepository _userRepository;
    private readonly InMemoryUserInterestRepository _interestRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly UserProfileService _service;

    public UserProfileServiceAvatarBioTests()
    {
        _sharedStore = new InMemorySharedStore();
        _userRepository = new InMemoryUserRepository(_sharedStore);
        _interestRepository = new InMemoryUserInterestRepository(_sharedStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new UserProfileService(
            _userRepository,
            _interestRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task UpdateBioAsync_WithValidBio_UpdatesBio()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        var bio = "Esta é minha biografia.";

        var updatedUser = await _service.UpdateBioAsync(user.Id, bio, CancellationToken.None);

        Assert.Equal(bio, updatedUser.Bio);
    }

    [Fact]
    public async Task UpdateBioAsync_WithNull_RemovesBio()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        await _service.UpdateBioAsync(user.Id, "Bio inicial", CancellationToken.None);

        var updatedUser = await _service.UpdateBioAsync(user.Id, null, CancellationToken.None);

        Assert.Null(updatedUser.Bio);
    }

    [Fact]
    public async Task UpdateBioAsync_WithEmptyString_RemovesBio()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        await _service.UpdateBioAsync(user.Id, "Bio inicial", CancellationToken.None);

        var updatedUser = await _service.UpdateBioAsync(user.Id, "", CancellationToken.None);

        Assert.Null(updatedUser.Bio);
    }

    [Fact]
    public async Task UpdateBioAsync_WithMaxLength_Accepts()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        var bio = new string('a', 500); // Exatamente 500 caracteres

        var updatedUser = await _service.UpdateBioAsync(user.Id, bio, CancellationToken.None);

        Assert.Equal(bio, updatedUser.Bio);
    }

    [Fact]
    public async Task UpdateBioAsync_WithExceedsMaxLength_ThrowsArgumentException()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        var bio = new string('a', 501); // Mais de 500 caracteres

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateBioAsync(user.Id, bio, CancellationToken.None));

        Assert.Contains("500", ex.Message);
    }

    [Fact]
    public async Task UpdateBioAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var bio = "Minha bio";

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateBioAsync(userId, bio, CancellationToken.None));

        Assert.Contains("User", ex.Message);
        Assert.Contains(userId.ToString(), ex.Message);
    }

    [Fact]
    public async Task UpdateBioAsync_TrimsWhitespace()
    {
        var user = await CreateAndAddUserAsync("Test User", "test@example.com", "111.111.111-11");
        var bio = "  Minha bio com espaços  ";

        var updatedUser = await _service.UpdateBioAsync(user.Id, bio, CancellationToken.None);

        Assert.Equal("Minha bio com espaços", updatedUser.Bio);
    }

    [Fact]
    public async Task UpdateAvatarAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var mediaAssetId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAvatarAsync(userId, mediaAssetId, CancellationToken.None));

        Assert.Contains("User", ex.Message);
        Assert.Contains(userId.ToString(), ex.Message);
    }

    private async Task<User> CreateAndAddUserAsync(string displayName, string email, string cpf)
    {
        var id = Guid.NewGuid();
        var user = new User(
            id,
            displayName,
            email,
            cpf,
            null,
            null,
            null,
            "google",
            "ext-" + id.ToString("N")[..8],
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);
        return user;
    }
}
