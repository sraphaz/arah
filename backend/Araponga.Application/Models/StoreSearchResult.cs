using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Models;

public sealed record StoreSearchResult(
    Store Store,
    double AverageRating);
