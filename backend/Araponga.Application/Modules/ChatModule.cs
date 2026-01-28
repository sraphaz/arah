using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Chat - serviços de chat e mensagens.
/// Depende de Core.
/// </summary>
public sealed class ChatModule : ModuleBase
{
    public override string Id => "Chat";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ChatService>();
    }
}
