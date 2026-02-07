using Arah.Bff.Contracts.Journeys.Common;

namespace Arah.Bff.Contracts.Journeys.Onboarding;

public sealed record TerritoryFeedInitialDto(
    IReadOnlyList<TerritoryFeedItemDto> Items,
    JourneyPaginationDto Pagination);
