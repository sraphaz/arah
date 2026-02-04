using System.Net;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Chat;
using Araponga.Domain.Users;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class ChatScenariosTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ResidentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task TerritoryChannels_CanListAndSendMessage()
    {
        using var factory = new ApiFactory();
        var sharedStore = factory.GetSharedStore();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        // Tornar usuário verificado (gate do chat)
        var user = sharedStore.Users.Single(u => u.Id == ResidentUserId);
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Verified, DateTime.UtcNow);

        var channels = await client.GetFromJsonAsync<ListChannelsResponse>(
            $"api/v1/territories/{ActiveTerritoryId}/chat/channels");

        Assert.NotNull(channels);
        Assert.True(channels!.Channels.Count >= 1);

        var publicChannel = channels.Channels.FirstOrDefault(c => c.Kind == "TERRITORYPUBLIC");
        Assert.NotNull(publicChannel);

        var send = await client.PostAsJsonAsync(
            $"api/v1/chat/conversations/{publicChannel!.Id}/messages",
            new SendMessageRequest("Olá canal público"));
        Assert.Equal(HttpStatusCode.Created, send.StatusCode);

        var messages = await client.GetFromJsonAsync<ListMessagesResponse>(
            $"api/v1/chat/conversations/{publicChannel.Id}/messages?limit=10");
        Assert.NotNull(messages);
        Assert.Contains(messages!.Messages, m => m.Text == "Olá canal público");
    }

    [Fact]
    public async Task Groups_CanCreateApproveAndChat()
    {
        using var factory = new ApiFactory();
        var sharedStore = factory.GetSharedStore();
        using var client = factory.CreateClient();

        var residentToken = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, residentToken);

        // Tornar usuário verificado para criar/enviar mensagem
        var user = sharedStore.Users.Single(u => u.Id == ResidentUserId);
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Verified, DateTime.UtcNow);

        var createGroup = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/chat/groups",
            new CreateGroupRequest("Grupo teste"));
        Assert.Equal(HttpStatusCode.Created, createGroup.StatusCode);

        var created = await createGroup.Content.ReadFromJsonAsync<ConversationResponse>();
        Assert.NotNull(created);
        Assert.Equal("GROUP", created!.Kind);
        Assert.Equal("PENDINGAPPROVAL", created.Status);

        // Aprovar como curador
        var curatorToken = await AuthTestHelper.LoginForTokenAsync(client, "google", "curator-external");
        AuthTestHelper.SetAuthHeader(client, curatorToken);

        var approve = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/chat/groups/{created.Id}/approve",
            null);
        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);

        var approved = await approve.Content.ReadFromJsonAsync<ConversationResponse>();
        Assert.NotNull(approved);
        Assert.Equal("ACTIVE", approved!.Status);

        // Enviar mensagem como dono
        AuthTestHelper.SetAuthHeader(client, residentToken);
        var send = await client.PostAsJsonAsync(
            $"api/v1/chat/conversations/{created.Id}/messages",
            new SendMessageRequest("Olá grupo"));
        Assert.Equal(HttpStatusCode.Created, send.StatusCode);
    }

}

