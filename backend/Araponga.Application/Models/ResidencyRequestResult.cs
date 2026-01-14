using Araponga.Domain.Social.JoinRequests;

namespace Araponga.Application.Models;

public sealed record ResidencyRequestResult(
    bool Created,
    TerritoryJoinRequest JoinRequest);

