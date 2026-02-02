namespace Araponga.Application.Interfaces;

/// <summary>
/// Registry de módulos para gerenciar registro de serviços modulares
/// </summary>
public interface IModuleRegistry
{
    void Apply(Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration);
}
