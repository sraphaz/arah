namespace Araponga.Modules.Moderation.Domain.Work;

public enum WorkItemStatus
{
    Pending = 1,
    AutoProcessed = 2,
    RequiresHumanReview = 3,
    Completed = 4,
    Cancelled = 5
}
