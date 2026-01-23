using Araponga.Application.Events;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Araponga.Api.HealthChecks;

public sealed class EventBusHealthCheck : IHealthCheck
{
    private readonly IEventBus _eventBus;

    public EventBusHealthCheck(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _eventBus is null
                ? HealthCheckResult.Unhealthy("Event bus não disponível.")
                : HealthCheckResult.Healthy("Event bus disponível."));
    }
}
