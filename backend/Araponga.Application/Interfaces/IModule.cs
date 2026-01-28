namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para módulos que registram suas próprias infraestruturas (DbContexts, repositórios, etc.)
/// </summary>
public interface IModule
{
    void RegisterServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration);
}
