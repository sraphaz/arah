using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Core - serviços fundamentais da aplicação.
/// Este módulo é obrigatório e não pode ser desabilitado.
/// </summary>
public sealed class CoreModule : ModuleBase
{
    public override string Id => "Core";
    public override string[] DependsOn => Array.Empty<string>();
    public override bool IsRequired => true;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddScoped<MembershipAccessRules>();
        // Nota: CurrentUserAccessor é registrado em AddSharedApplicationServices (está no Api)

        // Territory services
        services.AddScoped<TerritoryService>();
        services.AddScoped<ActiveTerritoryService>();
        services.AddScoped<HealthService>();

        // Auth services
        services.AddScoped<AuthService>();
        services.AddScoped<PasswordResetService>();

        // Membership services
        services.AddScoped<MembershipService>();
        services.AddScoped<ResidencyRequestService>();
        services.AddScoped<JoinRequestService>();

        // User services
        services.AddScoped<UserPreferencesService>();
        services.AddScoped<UserProfileService>();
        services.AddScoped<UserProfileStatsService>();

        // Verification and account management
        services.AddScoped<DataExportService>();
        services.AddScoped<AccountDeletionService>();

        // Terms and Privacy
        services.AddScoped<TermsOfServiceService>();
        services.AddScoped<TermsAcceptanceService>();
        services.AddScoped<PrivacyPolicyService>();
        services.AddScoped<PrivacyPolicyAcceptanceService>();
        services.AddScoped<PolicyRequirementService>();

        // AccessEvaluator (registrado depois para permitir injeção dos serviços de políticas)
        services.AddScoped<AccessEvaluator>(sp =>
        {
            return new AccessEvaluator(
                sp.GetRequiredService<ITerritoryMembershipRepository>(),
                sp.GetRequiredService<IMembershipCapabilityRepository>(),
                sp.GetRequiredService<ISystemPermissionRepository>(),
                sp.GetRequiredService<MembershipAccessRules>(),
                sp.GetRequiredService<IDistributedCacheService>(),
                sp.GetService<CacheMetricsService>(),
                sp.GetService<PolicyRequirementService>(),
                sp.GetService<TermsAcceptanceService>(),
                sp.GetService<PrivacyPolicyAcceptanceService>());
        });
    }
}
