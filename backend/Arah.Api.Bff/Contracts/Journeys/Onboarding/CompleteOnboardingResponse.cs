namespace Arah.Bff.Contracts.Journeys.Onboarding;

public sealed record CompleteOnboardingResponse(
    UserSummary User,
    TerritorySummary Territory,
    TerritoryFeedInitialDto InitialFeed,
    IReadOnlyList<SuggestedAction> SuggestedActions);
