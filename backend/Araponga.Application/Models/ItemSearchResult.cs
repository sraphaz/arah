using Araponga.Domain.Marketplace;

namespace Araponga.Application.Models;

public sealed record ItemSearchResult(
    StoreItem Item,
    double AverageRating);
