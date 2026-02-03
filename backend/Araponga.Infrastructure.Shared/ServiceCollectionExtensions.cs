using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Infrastructure.Shared.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Infrastructure.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        // Registrar SharedDbContext (disponível para uso direto; IUnitOfWork é registrado na API como CompositeUnitOfWork)
        services.AddDbContext<SharedDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        return services;
    }

    /// <summary>
    /// Registra repositórios Postgres que usam SharedDbContext (fonte da verdade em Shared).
    /// Chamado pela API após AddSharedInfrastructure quando Persistence:Provider = Postgres.
    /// </summary>
    public static IServiceCollection AddSharedCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositórios core que usam apenas entidades em Shared/Postgres/Entities
        services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<ITerritoryMembershipRepository, PostgresTerritoryMembershipRepository>();
        services.AddScoped<ITerritoryJoinRequestRepository, PostgresTerritoryJoinRequestRepository>();
        services.AddScoped<IUserPreferencesRepository, PostgresUserPreferencesRepository>();
        services.AddScoped<IUserInterestRepository, PostgresUserInterestRepository>();
        services.AddScoped<IVotingRepository, PostgresVotingRepository>();
        services.AddScoped<IVoteRepository, PostgresVoteRepository>();
        services.AddScoped<ITerritoryCharacterizationRepository, PostgresTerritoryCharacterizationRepository>();
        services.AddScoped<IMembershipSettingsRepository, PostgresMembershipSettingsRepository>();
        services.AddScoped<IMembershipCapabilityRepository, PostgresMembershipCapabilityRepository>();
        services.AddScoped<ISystemPermissionRepository, PostgresSystemPermissionRepository>();
        services.AddScoped<ISystemConfigRepository, PostgresSystemConfigRepository>();
        services.AddScoped<ITermsOfServiceRepository, PostgresTermsOfServiceRepository>();
        services.AddScoped<ITermsAcceptanceRepository, PostgresTermsAcceptanceRepository>();
        services.AddScoped<IPrivacyPolicyRepository, PostgresPrivacyPolicyRepository>();
        services.AddScoped<IPrivacyPolicyAcceptanceRepository, PostgresPrivacyPolicyAcceptanceRepository>();
        services.AddScoped<IUserDeviceRepository, PostgresUserDeviceRepository>();

        return services;
    }

    /// <summary>
    /// Registra InMemorySharedStore e repositórios InMemory core que usam esse store.
    /// Chamado pela API quando Persistence:Provider = InMemory (antes de AddInMemoryRepositories).
    /// </summary>
    public static IServiceCollection AddSharedInMemoryRepositories(this IServiceCollection services)
    {
        services.AddSingleton<InMemorySharedStore>();
        services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddSingleton<ITerritoryJoinRequestRepository, InMemoryTerritoryJoinRequestRepository>();
        services.AddSingleton<IUserPreferencesRepository, InMemoryUserPreferencesRepository>();
        services.AddSingleton<IUserInterestRepository, InMemoryUserInterestRepository>();
        services.AddSingleton<IVotingRepository, InMemoryVotingRepository>();
        services.AddSingleton<IVoteRepository, InMemoryVoteRepository>();
        services.AddSingleton<ITerritoryCharacterizationRepository, InMemoryTerritoryCharacterizationRepository>();
        services.AddSingleton<IMembershipSettingsRepository, InMemoryMembershipSettingsRepository>();
        services.AddSingleton<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddSingleton<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddSingleton<ISystemConfigRepository, InMemorySystemConfigRepository>();
        services.AddSingleton<ITermsOfServiceRepository, InMemoryTermsOfServiceRepository>();
        services.AddSingleton<ITermsAcceptanceRepository, InMemoryTermsAcceptanceRepository>();
        services.AddSingleton<IPrivacyPolicyRepository, InMemoryPrivacyPolicyRepository>();
        services.AddSingleton<IPrivacyPolicyAcceptanceRepository, InMemoryPrivacyPolicyAcceptanceRepository>();
        services.AddSingleton<IUserDeviceRepository, InMemoryUserDeviceRepository>();
        return services;
    }
}
