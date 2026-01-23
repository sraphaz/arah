using System.Net;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class AuthControllerForgotPasswordTests
{
    [Fact]
    public async Task ForgotPassword_ReturnsSuccess_WhenEmailIsProvided()
    {
        // Arrange
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var dataStore = factory.GetDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);

        // Criar usuário com email
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext123",
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var request = new ForgotPasswordRequest("test@example.com");

        // Act
        var response = await client.PostAsJsonAsync("api/v1/auth/forgot-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("message", content);
    }

    [Fact]
    public async Task ForgotPassword_ReturnsBadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest("");

        // Act
        var response = await client.PostAsJsonAsync("api/v1/auth/forgot-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_ReturnsSuccess_EvenWhenEmailDoesNotExist()
    {
        // Arrange
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest("nonexistent@example.com");

        // Act
        var response = await client.PostAsJsonAsync("api/v1/auth/forgot-password", request);

        // Assert - Security best practice: sempre retornar sucesso
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
