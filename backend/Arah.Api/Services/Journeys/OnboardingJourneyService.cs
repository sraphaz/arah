using Arah.Api.Contracts.Journeys.Onboarding;
using Arah.Api.Services.Journeys.Backend;
using Arah.Application.Interfaces.Geo;
using Arah.Application.Services;
using Arah.Domain.Territories;
using Microsoft.Extensions.Logging;

namespace Arah.Api.Services.Journeys;

public sealed class OnboardingJourneyService : IOnboardingJourneyService
{
    private readonly IOnboardingJourneyBackend _backend;
    private readonly IMunicipalityTerritoryProvisioningService _municipalityProvisioning;
    private readonly ITerritoryProposalService _territoryProposal;
    private readonly ILogger<OnboardingJourneyService> _logger;

    public OnboardingJourneyService(
        IOnboardingJourneyBackend backend,
        IMunicipalityTerritoryProvisioningService municipalityProvisioning,
        ITerritoryProposalService territoryProposal,
        ILogger<OnboardingJourneyService> logger)
    {
        _backend = backend;
        _municipalityProvisioning = municipalityProvisioning;
        _territoryProposal = territoryProposal;
        _logger = logger;
    }

    /// <summary>
    /// Conclui o onboarding: o usuário só se torna visitante do território neste momento.
    /// Esta tela aparece apenas após login/cadastro quando ainda não há preferência de território salva;
    /// se já houvesse território selecionado, o app iria direto para o feed sem pedir email, senha ou seleção.
    /// </summary>
    public async Task<CompleteOnboardingResponse?> CompleteOnboardingAsync(
        Guid userId,
        string sessionId,
        Guid selectedTerritoryId,
        CancellationToken cancellationToken = default)
    {
        var territory = await _backend.GetTerritoryByIdAsync(selectedTerritoryId, cancellationToken);
        if (territory is null)
            return null;

        var stored = await _backend.SetActiveTerritoryAsync(sessionId, selectedTerritoryId, cancellationToken);
        if (!stored)
            return null;

        // Único momento em que o usuário se torna visitante: ao concluir a seleção de território nesta tela.
        var membership = await _backend.EnterAsVisitorAsync(userId, selectedTerritoryId, cancellationToken);
        if (membership is null)
            return null;

        var user = await _backend.GetUserByIdAsync(userId, cancellationToken);
        var displayName = user?.DisplayName ?? "Usuário";
        var email = (string?)null; // Backend contract can be extended with Email if needed

        var pagedFeed = await _backend.ListFeedPagedAsync(selectedTerritoryId, userId, 1, 20, cancellationToken);
        var postIds = pagedFeed.Items.Select(p => p.Id).ToList();
        var counts = await _backend.GetCountsByPostIdsAsync(postIds, cancellationToken);
        var items = new List<TerritoryFeedItemDto>();
        foreach (var post in pagedFeed.Items)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new BackendPostCounts(0, 0));
            items.Add(new TerritoryFeedItemDto(
                post.Id,
                post.Title,
                post.Content,
                post.Type,
                post.CreatedAtUtc,
                post.Tags?.Count > 0 ? post.Tags : null,
                postCounts.LikeCount,
                postCounts.ShareCount,
                0,
                null));
        }

        const int maxInt32 = int.MaxValue;
        var totalCount = pagedFeed.TotalCount > maxInt32 ? maxInt32 : pagedFeed.TotalCount;
        var totalPages = pagedFeed.TotalPages > maxInt32 ? maxInt32 : pagedFeed.TotalPages;
        var paginationDto = new Arah.Api.Contracts.Journeys.Common.JourneyPaginationDto(
            pagedFeed.PageNumber,
            pagedFeed.PageSize,
            totalCount,
            totalPages,
            pagedFeed.HasPreviousPage,
            pagedFeed.HasNextPage);

        var initialFeed = new TerritoryFeedInitialDto(items, paginationDto);
        var userSummary = new UserSummary(userId, displayName, email, membership.Role);
        var territorySummary = new TerritorySummary(territory.Id, territory.Name, territory.Description, territory.Active);
        var suggestedActions = new List<SuggestedAction>
        {
            new("REQUEST_RESIDENCY", "Solicitar Residência", "Torne-se morador para acessar conteúdo exclusivo", "HIGH"),
            new("EXPLORE_MAP", "Explorar Mapa", "Descubra pontos de interesse no território", "MEDIUM")
        };

        return new CompleteOnboardingResponse(userSummary, territorySummary, initialFeed, suggestedActions);
    }

    public async Task<SuggestedTerritoriesResponse> GetSuggestedTerritoriesAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default)
    {
        var territories = await _backend.GetTerritoriesNearbyAsync(latitude, longitude, radiusKm, 20, cancellationToken);
        var list = new List<TerritorySuggestionDto>();
        foreach (var t in territories)
        {
            var distanceKm = HaversineKm(latitude, longitude, t.Latitude, t.Longitude);
            list.Add(new TerritorySuggestionDto(t.Id, t.Name, t.Description, Math.Round(distanceKm, 2), t.Latitude, t.Longitude));
        }
        return new SuggestedTerritoriesResponse(list);
    }

    public async Task<SuggestMunicipalityResponse?> SuggestMunicipalityAsync(
        double latitude,
        double longitude,
        string? city,
        string? state,
        CancellationToken cancellationToken = default)
    {
        var result = await _municipalityProvisioning.SuggestOrCreateAsync(
            latitude,
            longitude,
            city,
            state,
            cancellationToken);

        if (result is null)
        {
            return null;
        }

        return new SuggestMunicipalityResponse(
            result.TerritoryId,
            result.Name,
            result.City,
            result.StateCode,
            result.Description,
            result.DistanceKm,
            result.Latitude,
            result.Longitude,
            result.IbgeCode,
            result.Created);
    }

    public async Task<ProposeTerritoryResponse?> ProposeTerritoryAsync(
        Guid proposerUserId,
        ProposeTerritoryRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TerritoryBoundaryPoint>? polygon = null;
        if (request.BoundaryPolygon is { Count: >= 3 })
        {
            polygon = request.BoundaryPolygon
                .Select(p => new TerritoryBoundaryPoint(p.Latitude, p.Longitude))
                .ToList();
        }

        var result = await _territoryProposal.ProposeAsync(
            proposerUserId,
            request.Name ?? string.Empty,
            request.City,
            request.State,
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            polygon,
            request.ParentTerritoryId,
            cancellationToken);

        if (result is null)
        {
            return null;
        }

        return new ProposeTerritoryResponse(
            result.TerritoryId,
            result.Name,
            result.City,
            result.StateCode,
            result.Description,
            result.DistanceKm,
            result.Latitude,
            result.Longitude,
            result.Status.ToString(),
            result.IsPending,
            result.Created);
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
