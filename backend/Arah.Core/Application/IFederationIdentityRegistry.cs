namespace Arah.Core.Application;

using Arah.Core.Domain;

public interface IFederationIdentityRegistry
{
    FederatedGlobalUser Provision(Guid homeInstanceId, string displayName);
    FederatedGlobalUser? GetByGlobalUserId(Guid globalUserId);
}

public sealed class InMemoryFederationIdentityRegistry : IFederationIdentityRegistry
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, FederatedGlobalUser> _users = new();

    public FederatedGlobalUser Provision(Guid homeInstanceId, string displayName)
    {
        var user = new FederatedGlobalUser(Guid.NewGuid(), homeInstanceId, displayName, DateTimeOffset.UtcNow);
        _users[user.GlobalUserId] = user;
        return user;
    }

    public FederatedGlobalUser? GetByGlobalUserId(Guid globalUserId) =>
        _users.TryGetValue(globalUserId, out var user) ? user : null;
}
