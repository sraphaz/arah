namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record CreatePostJourneyResponse(
    TerritoryFeedPostDto Post,
    IReadOnlyList<string> MediaUrls,
    CreatePostSuggestionsDto? Suggestions);

public sealed record CreatePostSuggestionsDto(
    IReadOnlyList<TerritoryFeedPostDto>? SimilarPosts,
    IReadOnlyList<string>? SuggestedTags);
