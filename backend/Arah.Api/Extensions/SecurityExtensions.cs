using Arah.Api.Configuration;
using Arah.Api.Security;
using Arah.Application.Configuration;
using Arah.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Arah.Api.Extensions;

/// <summary>
/// Segurança da aplicação: validação da chave JWT, binding de options, CORS,
/// antiforgery, autenticação/autorização e opções de HSTS.
/// </summary>
public static class SecurityExtensions
{
    public static IServiceCollection AddArahSecurity(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Configuration Validation - JWT Secret
        var jwtSigningKey = configuration["Jwt:SigningKey"] ?? configuration["JWT__SIGNINGKEY"];
        if (string.IsNullOrWhiteSpace(jwtSigningKey))
        {
            throw new InvalidOperationException(
                "JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY or appsettings.json. " +
                "Never leave this empty.");
        }

        if (jwtSigningKey == "dev-only-change-me")
        {
            if (environment.IsProduction())
            {
                throw new InvalidOperationException(
                    "JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY in production. " +
                    "Never use the default value in production.");
            }
            else if (!environment.IsEnvironment("Testing"))
            {
                Log.Warning("Using default JWT SigningKey. This should be changed in production.");
            }
        }

        // Validate secret strength (minimum 32 characters for production)
        if (environment.IsProduction() && jwtSigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                $"JWT SigningKey must be at least 32 characters long in production. Current length: {jwtSigningKey.Length}");
        }

        // Configuration
        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtOptions>(jwtSection);
        services.Configure<PresencePolicyOptions>(configuration.GetSection("PresencePolicy"));
        services.Configure<Arah.Api.Configuration.ClientCredentialsOptions>(configuration.GetSection("ClientCredentials"));
        services.Configure<PasswordResetOptions>(configuration.GetSection("Auth:PasswordReset"));

        // CORS Configuration
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                             (environment.IsDevelopment() ? new[] { "*" } : Array.Empty<string>());

        if (environment.IsProduction())
        {
            if (allowedOrigins == null || allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
            {
                throw new InvalidOperationException(
                    "Cors:AllowedOrigins must be configured with specific origins in production. " +
                    "Wildcard (*) is not allowed in production.");
            }
        }

        services.AddCors(options =>
        {
            options.AddPolicy("Default", corsBuilder =>
            {
                if (allowedOrigins.Contains("*"))
                {
                    corsBuilder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                }
                else
                {
                    corsBuilder.WithOrigins(allowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .SetPreflightMaxAge(TimeSpan.FromHours(24));
                }
            });
        });

        // Anti-forgery tokens for CSRF protection
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-Token";
            options.Cookie.Name = "__Host-CSRF";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Authentication - Configurar esquema padrão para Forbid()
        // Nota: A autenticação JWT é feita via middleware customizado, mas precisamos de um esquema padrão
        // para que ForbidResult funcione corretamente quando retornamos Forbid()
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
            options.DefaultForbidScheme = "Bearer";
        })
        .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
            "Bearer", _ => { });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SystemAdmin", policy =>
                policy.Requirements.Add(new Arah.Api.Security.SystemAdminRequirement()));
        });
        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Arah.Api.Security.SystemAdminAuthorizationHandler>();

        // Configure HSTS (deve ser configurado antes de Build())
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);

            // Only in production
            if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
            {
                options.ExcludedHosts.Clear();
            }
        });

        return services;
    }
}
