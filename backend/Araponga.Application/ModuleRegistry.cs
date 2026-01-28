using Araponga.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Araponga.Application;

public sealed class ModuleRegistry : IModuleRegistry
{
    private readonly IModule[] _modules;
    private readonly ILogger<ModuleRegistry> _logger;

    public ModuleRegistry(IModule[] modules, ILogger<ModuleRegistry> logger)
    {
        _modules = modules;
        _logger = logger;
    }

    public void Apply(IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in _modules)
        {
            try
            {
                _logger.LogInformation("Registrando módulo: {ModuleType}", module.GetType().Name);
                module.RegisterServices(services, configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar módulo {ModuleType}", module.GetType().Name);
                throw;
            }
        }
    }
}
