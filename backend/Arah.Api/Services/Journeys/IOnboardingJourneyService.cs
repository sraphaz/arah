using Arah.Api.Contracts.Journeys.Onboarding;

namespace Arah.Api.Services.Journeys;

public interface IOnboardingJourneyService
{
    Task<CompleteOnboardingResponse?> CompleteOnboardingAsync(
        Guid userId,
        string sessionId,
        Guid selectedTerritoryId,
        CancellationToken cancellationToken = default);

    Task<SuggestedTerritoriesResponse> GetSuggestedTerritoriesAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default);

    Task<SuggestMunicipalityResponse?> SuggestMunicipalityAsync(
        double latitude,
        double longitude,
        string? city,
        string? state,
        CancellationToken cancellationToken = default);

    Task<ProposeTerritoryResponse?> ProposeTerritoryAsync(
        Guid proposerUserId,
        ProposeTerritoryRequest request,
        CancellationToken cancellationToken = default);
}
