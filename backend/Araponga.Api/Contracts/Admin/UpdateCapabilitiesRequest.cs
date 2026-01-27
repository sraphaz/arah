namespace Araponga.Api.Contracts.Admin;

public sealed class UpdateCapabilitiesRequest
{
    public List<string> Capabilities { get; set; } = new();
}
