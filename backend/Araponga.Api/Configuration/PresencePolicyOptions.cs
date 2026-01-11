namespace Araponga.Api.Configuration;

public enum PresencePolicy
{
    None,
    ResidentOnly,
    VisitorAndResident
}

public sealed class PresencePolicyOptions
{
    public PresencePolicy Policy { get; set; } = PresencePolicy.ResidentOnly;
}
