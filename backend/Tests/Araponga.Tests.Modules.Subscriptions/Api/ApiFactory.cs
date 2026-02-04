using Araponga.Api;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Tests.Modules.Subscriptions.Api;

/// <summary>
/// Factory para criar instâncias da API para testes do módulo Subscriptions.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private InMemoryDataStore? _dataStore;
    private InMemorySharedStore? _sharedStore;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        Environment.SetEnvironmentVariable("JWT__SIGNINGKEY", "ZPq7X8Y2m0bH3kLwQ1fRrC8n5Eo9Tt4K6SxDVaJpM=");
        Environment.SetEnvironmentVariable("RateLimiting__PermitLimit", "1000");
        Environment.SetEnvironmentVariable("Persistence__Provider", "InMemory");
        Environment.SetEnvironmentVariable("Persistence__ApplyMigrations", "false");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(InMemoryDataStore));
            if (descriptor != null) services.Remove(descriptor);
            var sharedDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(InMemorySharedStore));
            if (sharedDescriptor != null) services.Remove(sharedDescriptor);
            _sharedStore = new InMemorySharedStore();
            services.AddSingleton(_sharedStore);
            _dataStore = new InMemoryDataStore();
            services.AddSingleton(_dataStore);
        });
    }

    public InMemoryDataStore GetDataStore() =>
        _dataStore ??= Services.GetRequiredService<InMemoryDataStore>();

    public InMemorySharedStore GetSharedStore() =>
        _sharedStore ??= Services.GetRequiredService<InMemorySharedStore>();

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _dataStore = null; _sharedStore = null; }
        base.Dispose(disposing);
    }
}
