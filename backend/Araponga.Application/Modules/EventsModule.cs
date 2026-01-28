using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Events - serviços de eventos comunitários.
/// Depende de Core. Pode integrar com Feed (ex.: eventos no feed).
/// </summary>
public sealed class EventsModule : ModuleBase
{
    public override string Id => "Events";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<EventsService>();
    }
}
