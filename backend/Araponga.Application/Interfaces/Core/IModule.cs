using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para módulos que registram suas próprias infraestruturas (DbContexts, repositórios, etc.)
/// </summary>
public interface IModule
{
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}
