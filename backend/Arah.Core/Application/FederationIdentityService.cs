namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class FederationIdentityService
{
    private readonly IFederationIdentityRegistry _identities;
    private readonly ICoreInstanceRegistry _instances;

    public FederationIdentityService(IFederationIdentityRegistry identities, ICoreInstanceRegistry instances)
    {
        _identities = identities;
        _instances = instances;
    }

    public FederatedGlobalUser Provision(Guid homeInstanceId, string displayName)
    {
        if (_instances.GetById(homeInstanceId) is null)
        {
            throw new InvalidOperationException($"Instance {homeInstanceId} is not registered");
        }

        return _identities.Provision(homeInstanceId, displayName);
    }

    public FederatedGlobalUser? Resolve(Guid globalUserId) =>
        _identities.GetByGlobalUserId(globalUserId);
}
