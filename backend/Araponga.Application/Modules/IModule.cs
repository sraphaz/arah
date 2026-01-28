using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Interface que define um módulo da aplicação.
/// Cada módulo registra apenas os serviços da sua funcionalidade.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Identificador único do módulo (ex.: "Core", "Feed", "Marketplace").
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Módulos que precisam estar habilitados antes deste módulo.
    /// Ex.: Feed depende de Core.
    /// </summary>
    string[] DependsOn { get; }

    /// <summary>
    /// Se true, o módulo não pode ser desabilitado (ex.: Core).
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    /// Registra os serviços do módulo no container de DI.
    /// Não registra repositórios nem infraestrutura global (isso é feito no host).
    /// </summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}
