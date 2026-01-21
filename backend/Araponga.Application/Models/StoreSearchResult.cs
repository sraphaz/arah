using Araponga.Domain.Marketplace;

namespace Araponga.Application.Models;

public sealed record StoreSearchResult(
    Store Store,
    double AverageRating);
