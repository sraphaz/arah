using Arah.Application.Interfaces;
using Arah.Application.Services;
using Arah.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Arah.Infrastructure.Hosting;

/// <summary>
/// Cria usuário SystemAdmin piloto em Postgres quando Pilot:BootstrapAdminEnabled=true (staging/FASE54).
/// </summary>
public sealed class PilotAdminBootstrapHostedService : IHostedService
{
    private static readonly Guid PilotAdminUserId = Guid.Parse("ffffffff-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PilotAdminBootstrapHostedService> _logger;

    public PilotAdminBootstrapHostedService(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger<PilotAdminBootstrapHostedService> logger)
    {
        _services = services;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_configuration.GetValue<bool>("Pilot:BootstrapAdminEnabled"))
        {
            return;
        }

        var provider = _configuration["Pilot:BootstrapAdminProvider"] ?? "google";
        var externalId = _configuration["Pilot:BootstrapAdminExternalId"] ?? "admin-external";
        var displayName = _configuration["Pilot:BootstrapAdminDisplayName"] ?? "Pilot System Admin";
        var email = _configuration["Pilot:BootstrapAdminEmail"] ?? "pilot-admin@arah.local";

        using var scope = _services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var permissions = scope.ServiceProvider.GetRequiredService<SystemPermissionService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var existing = await users.GetByAuthProviderAsync(provider, externalId, cancellationToken);
        var userId = existing?.Id ?? PilotAdminUserId;
        var changed = existing is null;

        if (existing is null)
        {
            var user = new User(
                userId,
                displayName,
                email,
                cpf: "000.000.000-00",
                foreignDocument: null,
                phoneNumber: null,
                address: null,
                authProvider: provider,
                externalId: externalId,
                createdAtUtc: DateTime.UtcNow);

            await users.AddAsync(user, cancellationToken);
            _logger.LogInformation(
                "Pilot bootstrap: created admin user {Provider}/{ExternalId}",
                provider,
                externalId);
        }

        var grant = await permissions.GrantAsync(
            userId,
            SystemPermissionType.SystemAdmin,
            grantedByUserId: userId,
            cancellationToken);

        if (grant.IsSuccess)
        {
            changed = true;
            _logger.LogInformation("Pilot bootstrap: granted SystemAdmin to {UserId}", userId);
        }
        else if (existing is not null)
        {
            _logger.LogDebug("Pilot bootstrap: SystemAdmin already present for {UserId}", userId);
        }
        else
        {
            _logger.LogWarning(
                "Pilot bootstrap: failed to grant SystemAdmin — {Error}",
                grant.Error);
        }

        if (changed)
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
