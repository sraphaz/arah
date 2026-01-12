namespace Araponga.Application.Events;

public interface IEventHandler<in TEvent>
    where TEvent : IAppEvent
{
    Task HandleAsync(TEvent appEvent, CancellationToken cancellationToken);
}
