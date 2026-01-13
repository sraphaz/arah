using Araponga.Domain.Social.JoinRequests;

namespace Araponga.Application.Models;

public sealed record JoinRequestDecisionResult(
    bool Found,
    bool Forbidden,
    TerritoryJoinRequest? Request,
    bool Updated);
