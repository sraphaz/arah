namespace Araponga.Application.Events;

public interface IAppEvent
{
    DateTime OccurredAtUtc { get; }
}
