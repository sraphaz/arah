namespace Araponga.Api.Contracts.Users;

public sealed record UpdatePrivacyPreferencesRequest(
    string ProfileVisibility,      // "Public", "ResidentsOnly", "Private"
    string ContactVisibility,       // "Public", "ResidentsOnly", "Private"
    bool ShareLocation,
    bool ShowMemberships);
