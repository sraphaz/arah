using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração para POST auth/signup e POST auth/login (cadastro e login com e-mail/senha).
/// </summary>
public sealed class AuthControllerSignupLoginTests
{
    [Fact]
    public async Task SignUp_Returns200_WithToken_WhenValidRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new SignUpRequest("signup-new@example.com", "New User", "password123");

        var response = await client.PostAsJsonAsync("api/v1/auth/signup", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out _));
        Assert.True(json.TryGetProperty("refreshToken", out _));
        Assert.True(json.TryGetProperty("user", out var user));
        Assert.Equal("New User", user.GetProperty("displayName").GetString());
        Assert.Equal("signup-new@example.com", user.GetProperty("email").GetString());
    }

    [Fact]
    public async Task SignUp_Returns400_WhenEmailAlreadyExists()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var first = new SignUpRequest("existing-signup@example.com", "Existing", "pass123");
        var firstResponse = await client.PostAsJsonAsync("api/v1/auth/signup", first);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var request = new SignUpRequest("existing-signup@example.com", "Other", "otherpass");
        var response = await client.PostAsJsonAsync("api/v1/auth/signup", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("already registered", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SignUp_Returns400_WhenPasswordTooShort()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new SignUpRequest("short@example.com", "User", "12345");
        var response = await client.PostAsJsonAsync("api/v1/auth/signup", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("6", content);
    }

    [Fact]
    public async Task SignUp_Returns400_WhenEmailEmpty()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new SignUpRequest("", "User", "password123");
        var response = await client.PostAsJsonAsync("api/v1/auth/signup", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_Returns200_WithToken_WhenValidCredentials()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var signUpReq = new SignUpRequest("login-ok@example.com", "Login User", "mypassword");
        var signUpRes = await client.PostAsJsonAsync("api/v1/auth/signup", signUpReq);
        Assert.Equal(HttpStatusCode.OK, signUpRes.StatusCode);

        var request = new LoginRequest("login-ok@example.com", "mypassword");
        var response = await client.PostAsJsonAsync("api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out _));
        Assert.True(json.TryGetProperty("user", out var user));
        Assert.Equal("login-ok@example.com", user.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Login_Returns400_WhenWrongPassword()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var signUpReq = new SignUpRequest("wrong-pass@example.com", "User", "correct");
        var signUpRes = await client.PostAsJsonAsync("api/v1/auth/signup", signUpReq);
        Assert.Equal(HttpStatusCode.OK, signUpRes.StatusCode);

        var request = new LoginRequest("wrong-pass@example.com", "wrongpassword");
        var response = await client.PostAsJsonAsync("api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_Returns400_WhenUserNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new LoginRequest("nonexistent@example.com", "anypass");
        var response = await client.PostAsJsonAsync("api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_Returns400_WhenPasswordEmpty()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var signUpReq = new SignUpRequest("empty-pass@example.com", "User", "password");
        var signUpRes = await client.PostAsJsonAsync("api/v1/auth/signup", signUpReq);
        Assert.Equal(HttpStatusCode.OK, signUpRes.StatusCode);

        var request = new LoginRequest("empty-pass@example.com", "");
        var response = await client.PostAsJsonAsync("api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
