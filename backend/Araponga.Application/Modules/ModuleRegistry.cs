using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Modules;

/// <summary>
/// Implementação do registry de módulos.
/// Lê configuração Modules, valida dependências, ordena e registra serviços.
/// </summary>
public class ModuleRegistry : IModuleRegistry
{
    private readonly IEnumerable<IModule> _modules;
    private readonly ILogger<ModuleRegistry>? _logger;
    private readonly HashSet<string> _enabledModules = new();

    public ModuleRegistry(IEnumerable<IModule> modules, ILogger<ModuleRegistry>? logger = null)
    {
        _modules = modules ?? throw new ArgumentNullException(nameof(modules));
        _logger = logger;
    }

    public void Apply(IServiceCollection services, IConfiguration configuration)
    {
        _enabledModules.Clear();

        var modulesDict = _modules.ToDictionary(m => m.Id, m => m);
        var moduleConfig = configuration.GetSection("Modules");

        // Determinar quais módulos estão habilitados
        var enabledModules = new HashSet<string>();
        var moduleStates = new Dictionary<string, ModuleState>();

        foreach (var module in _modules)
        {
            var moduleSection = moduleConfig.GetSection(module.Id);
            var enabled = moduleSection.GetValue<bool?>("Enabled") ?? true; // Default: habilitado
            var required = moduleSection.GetValue<bool?>("Required") ?? module.IsRequired;

            moduleStates[module.Id] = new ModuleState
            {
                Module = module,
                Enabled = enabled,
                Required = required
            };

            if (enabled)
            {
                enabledModules.Add(module.Id);
            }
        }

        // Validação 1: módulos Required devem estar Enabled
        foreach (var (moduleId, state) in moduleStates)
        {
            if (state.Required && !state.Enabled)
            {
                var error = $"Módulo obrigatório '{moduleId}' não pode ser desabilitado.";
                _logger?.LogError(error);
                throw new InvalidOperationException(error);
            }
        }

        // Validação 2: se um módulo está Disabled, nenhum módulo que dependa dele pode estar Enabled (cascata)
        foreach (var (moduleId, state) in moduleStates)
        {
            if (!state.Enabled)
            {
                var dependents = _modules
                    .Where(m => m.DependsOn.Contains(moduleId))
                    .Select(m => m.Id);

                foreach (var dependent in dependents)
                {
                    if (enabledModules.Contains(dependent))
                    {
                        var error = $"Módulo '{dependent}' depende de '{moduleId}' que está desabilitado. Desabilite '{dependent}' ou habilite '{moduleId}'.";
                        _logger?.LogError(error);
                        throw new InvalidOperationException(error);
                    }
                }
            }
        }

        // Ordenação topológica por dependências
        var sortedModules = TopologicalSort(enabledModules, moduleStates);

        // Registrar serviços dos módulos habilitados na ordem correta
        foreach (var moduleId in sortedModules)
        {
            var state = moduleStates[moduleId];
            _logger?.LogInformation("Registrando serviços do módulo: {ModuleId}", moduleId);

            try
            {
                Araponga.Application.Metrics.ArapongaMetrics.ModuleRegistrationAttempts.Add(1, new KeyValuePair<string, object?>("module_id", moduleId));
                state.Module.RegisterServices(services, configuration);
                _enabledModules.Add(moduleId);
            }
            catch (Exception ex)
            {
                Araponga.Application.Metrics.ArapongaMetrics.ModuleRegistrationFailures.Add(1, new KeyValuePair<string, object?>("module_id", moduleId));
                _logger?.LogError(ex, "Falha ao registrar módulo: {ModuleId}", moduleId);
                throw;
            }
        }

        _logger?.LogInformation("Módulos habilitados: {Modules}", string.Join(", ", _enabledModules));
    }

    public bool IsModuleEnabled(string moduleId)
    {
        return _enabledModules.Contains(moduleId);
    }

    public IReadOnlySet<string> GetEnabledModules()
    {
        return _enabledModules;
    }

    public IReadOnlySet<string> GetRequiredModules()
    {
        return _modules
            .Where(m => m.IsRequired)
            .Select(m => m.Id)
            .ToHashSet();
    }

    private List<string> TopologicalSort(HashSet<string> enabledModules, Dictionary<string, ModuleState> moduleStates)
    {
        var result = new List<string>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        void Visit(string moduleId)
        {
            if (visiting.Contains(moduleId))
            {
                throw new InvalidOperationException($"Dependência circular detectada envolvendo o módulo '{moduleId}'.");
            }

            if (visited.Contains(moduleId) || !enabledModules.Contains(moduleId))
            {
                return;
            }

            visiting.Add(moduleId);
            var state = moduleStates[moduleId];

            // Visitar dependências primeiro
            foreach (var dependency in state.Module.DependsOn)
            {
                if (enabledModules.Contains(dependency))
                {
                    Visit(dependency);
                }
            }

            visiting.Remove(moduleId);
            visited.Add(moduleId);
            result.Add(moduleId);
        }

        foreach (var moduleId in enabledModules)
        {
            Visit(moduleId);
        }

        return result;
    }

    private class ModuleState
    {
        public IModule Module { get; set; } = null!;
        public bool Enabled { get; set; }
        public bool Required { get; set; }
    }
}
