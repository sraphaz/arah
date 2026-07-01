namespace Arah.Core;

using Arah.Core.Application;
using Arah.Core.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal sealed class CoreReleaseSeedHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<CoreReleaseSeedHostedService> _logger;

    public CoreReleaseSeedHostedService(IServiceProvider services, ILogger<CoreReleaseSeedHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var catalog = scope.ServiceProvider.GetRequiredService<ICoreReleaseCatalog>();
        if (catalog.ListByChannel("stable").Count > 0)
        {
            return Task.CompletedTask;
        }

        catalog.Publish(new CoreRelease(
            Guid.Parse("00000000-0000-4000-8000-000000000001"),
            "0.1.0",
            "stable",
            schemaVersion: 1,
            DateTimeOffset.UtcNow));
        _logger.LogInformation("Seeded default stable release 0.1.0");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
