using System.IO;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Araponga.Api.HealthChecks;

public sealed class StorageHealthCheck : IHealthCheck
{
    private readonly MediaStorageOptions _options;

    public StorageHealthCheck(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var provider = _options.Provider?.Trim();
        if (string.IsNullOrWhiteSpace(provider))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("MediaStorage provider não configurado."));
        }

        if (string.Equals(provider, "Local", StringComparison.OrdinalIgnoreCase))
        {
            var path = _options.LocalPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("MediaStorage LocalPath não configurado."));
            }

            return Task.FromResult(
                Directory.Exists(path)
                    ? HealthCheckResult.Healthy("Storage local disponível.")
                    : HealthCheckResult.Degraded($"Diretório de mídia não encontrado: {path}."));
        }

        if (string.Equals(provider, "S3", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(_options.S3BucketName) ||
                string.IsNullOrWhiteSpace(_options.S3Region) ||
                string.IsNullOrWhiteSpace(_options.S3AccessKeyId) ||
                string.IsNullOrWhiteSpace(_options.S3SecretAccessKey))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Configuração S3 incompleta."));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Configuração S3 presente."));
        }

        if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(_options.AzureBlobConnectionString) ||
                string.IsNullOrWhiteSpace(_options.AzureBlobContainerName))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Configuração Azure Blob incompleta."));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Configuração Azure Blob presente."));
        }

        return Task.FromResult(HealthCheckResult.Degraded($"Provider de storage desconhecido: {provider}."));
    }
}
