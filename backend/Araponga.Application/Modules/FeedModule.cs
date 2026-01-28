using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Feed - serviços de feed comunitário.
/// Depende de Core.
/// </summary>
public sealed class FeedModule : ModuleBase
{
    public override string Id => "Feed";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Feed services
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostEditService>();
        services.AddScoped<PostInteractionService>();
        services.AddScoped<PostFilterService>();
        services.AddScoped<FeedService>();
    }
}
