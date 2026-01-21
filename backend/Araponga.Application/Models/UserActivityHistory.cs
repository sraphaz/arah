using Araponga.Application.Common;

namespace Araponga.Application.Models;

public sealed record UserActivityHistory(
    PagedResult<UserPostActivity> Posts,
    PagedResult<UserEventActivity> Events,
    PagedResult<UserPurchaseActivity> Purchases,
    PagedResult<UserParticipationActivity> Participations);
