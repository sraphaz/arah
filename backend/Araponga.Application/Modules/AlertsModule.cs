using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Alerts - serviços de alertas.
/// Depende de Core.
/// </summary>
public sealed class AlertsModule : ModuleBase
{
    public override string Id => "Alerts";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Serviços específicos de alertas serão adicionados aqui quando implementados
    }
}
