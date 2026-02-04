using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Araponga.Application.Models;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração da API de conexões (círculo de amigos) e do fluxo de notificações (Outbox).
/// </summary>
public sealed class ConnectionsIntegrationTests
{
    [Fact]
    public async Task RequestConnection_ThenAccept_EnqueuesConnectionRequestAndAcceptedNotifications()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        AuthTestHelper.SetSessionId(client);

        var respA = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "conn-integration-user-a");
        AuthTestHelper.SetupAuthenticatedClient(client, respA.Token);

        var respB = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "conn-integration-user-b");
        var userAId = respA.User.Id;
        var userBId = respB.User.Id;

        AuthTestHelper.SetAuthHeader(client, respA.Token);
        var requestResponse = await client.PostAsJsonAsync(
            "api/v1/connections/request",
            new { TargetUserId = userBId });
        requestResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, requestResponse.StatusCode);

        var dataStore = factory.GetDataStore();
        var requestNotifications = dataStore.OutboxMessages
            .Where(m => m.Type == "notification.dispatch")
            .ToList();
        Assert.NotEmpty(requestNotifications);

        var requestPayload = JsonSerializer.Deserialize<NotificationDispatchPayload>(
            requestNotifications[^1].PayloadJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(requestPayload);
        Assert.Equal("connection.request", requestPayload.Kind);
        Assert.Single(requestPayload.Recipients);
        Assert.Equal(userBId, requestPayload.Recipients.First());

        var createdResponse = await requestResponse.Content.ReadFromJsonAsync<ConnectionCreatedResponse>();
        var connectionId = createdResponse!.Id;

        AuthTestHelper.SetAuthHeader(client, respB.Token);
        var acceptResponse = await client.PostAsync(
            $"api/v1/connections/{connectionId}/accept",
            null);
        acceptResponse.EnsureSuccessStatusCode();

        var acceptedNotifications = dataStore.OutboxMessages
            .Where(m => m.Type == "notification.dispatch")
            .ToList();
        Assert.True(acceptedNotifications.Count >= 2, "Expected at least connection.request and connection.accepted");

        var acceptedPayload = JsonSerializer.Deserialize<NotificationDispatchPayload>(
            acceptedNotifications[^1].PayloadJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(acceptedPayload);
        Assert.Equal("connection.accepted", acceptedPayload.Kind);
        Assert.Single(acceptedPayload.Recipients);
        Assert.Equal(userAId, acceptedPayload.Recipients.First());
    }

    private sealed record ConnectionCreatedResponse(
        Guid Id,
        Guid RequesterUserId,
        Guid TargetUserId,
        string Status,
        Guid? TerritoryId,
        DateTime RequestedAtUtc);
}
