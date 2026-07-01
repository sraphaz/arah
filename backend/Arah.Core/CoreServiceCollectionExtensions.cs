namespace Arah.Core;

using Arah.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddArahCore(this IServiceCollection services)
    {
        services.AddSingleton<ICoreInstanceRegistry, InMemoryCoreInstanceRegistry>();
        services.AddSingleton<ICoreReleaseCatalog, InMemoryCoreReleaseCatalog>();
        services.AddSingleton<ICoreDirectoryRegistry, InMemoryCoreDirectoryRegistry>();
        services.AddSingleton<IFederationIdentityRegistry, InMemoryFederationIdentityRegistry>();
        services.AddScoped<CoreInstanceService>();
        services.AddScoped<CoreDirectoryService>();
        services.AddScoped<FederationIdentityService>();
        return services;
    }

    public static IServiceCollection AddArahCoreSeedReleases(this IServiceCollection services)
    {
        services.AddSingleton<IHostedService, CoreReleaseSeedHostedService>();
        return services;
    }
}
