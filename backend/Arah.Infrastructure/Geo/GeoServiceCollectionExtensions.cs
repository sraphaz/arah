using Arah.Application.Interfaces.Geo;
using Arah.Infrastructure.Geo;
using Microsoft.Extensions.DependencyInjection;

namespace Arah.Infrastructure.Geo;

public static class GeoServiceCollectionExtensions
{
    public static IServiceCollection AddGeoIntegrationServices(this IServiceCollection services)
    {
        services.AddHttpClient<IIbgeBoundaryResolver, IbgeBoundaryResolver>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient<IReverseGeocodingService, NominatimReverseGeocodingService>(client =>
        {
            client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
            client.DefaultRequestHeaders.Add("User-Agent", "ArahPlatform/1.0 (territory-onboarding; contact@arah.local)");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
