using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Notifications - serviços de notificações push e email.
/// Depende de Core. Usa conector compartilhado de email.
/// </summary>
public sealed class NotificationsModule : ModuleBase
{
    public override string Id => "Notifications";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<PushNotificationService>();
        services.AddScoped<Araponga.Application.Services.Notifications.NotificationConfigService>();

        // Nota: IEmailSender é um conector compartilhado registrado no host.
    }
}
