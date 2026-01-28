using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Registry que gerencia módulos da aplicação.
/// </summary>
public interface IModuleRegistry
{
    /// <summary>
    /// Aplica a configuração de módulos: lê configuração, valida dependências,
    /// ordena por dependência e registra serviços dos módulos habilitados.
    /// </summary>
    void Apply(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Retorna se o módulo está habilitado após Apply.
    /// Permite que um módulo, em runtime, verifique se outro está ativo
    /// e habilite integrações opcionais (ex.: Subscriptions verifica Marketplace).
    /// </summary>
    bool IsModuleEnabled(string moduleId);

    /// <summary>
    /// Retorna a lista de IDs dos módulos habilitados.
    /// </summary>
    IReadOnlySet<string> GetEnabledModules();

    /// <summary>
    /// Retorna a lista de IDs dos módulos obrigatórios.
    /// </summary>
    IReadOnlySet<string> GetRequiredModules();
}
