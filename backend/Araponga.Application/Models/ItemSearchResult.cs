using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Models;

public sealed record ItemSearchResult(
    StoreItem Item,
    double AverageRating);
