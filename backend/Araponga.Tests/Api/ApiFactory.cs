using System.Collections.Generic;
using Araponga.Api;
using Araponga.Infrastructure.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Tests.Api;

/// <summary>
/// Factory para criar instâncias da API para testes.
/// IMPORTANTE: Cada instância cria um novo InMemoryDataStore isolado,
/// garantindo que os testes não compartilhem estado entre si.
/// 
/// Para garantir isolamento completo:
/// - Cada teste deve criar seu próprio ApiFactory usando 'using var factory = new ApiFactory()'
/// - Ou usar IClassFixture para compartilhar o factory apenas dentro da mesma classe de teste
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private InMemoryDataStore? _dataStore;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Configurar JWT secret via variáveis de ambiente (mais confiável que in-memory)
        // Usando o secret forte fornecido: ZPq7X8Y2m0bH3kLwQ1fRrC8n5Eo9Tt4K6SxDVaJpM=
        Environment.SetEnvironmentVariable("JWT__SIGNINGKEY", "ZPq7X8Y2m0bH3kLwQ1fRrC8n5Eo9Tt4K6SxDVaJpM=");
        Environment.SetEnvironmentVariable("RateLimiting__PermitLimit", "1000");
        Environment.SetEnvironmentVariable("RateLimiting__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__QueueLimit", "100");
        Environment.SetEnvironmentVariable("Cors__AllowedOrigins__0", "*");
        Environment.SetEnvironmentVariable("Persistence__Provider", "InMemory");
        Environment.SetEnvironmentVariable("Persistence__ApplyMigrations", "false");

        // Configurar appsettings.json do projeto de testes
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Carregar appsettings.json do projeto de testes
            var assemblyLocation = typeof(ApiFactory).Assembly.Location;
            var testProjectDir = Path.GetDirectoryName(assemblyLocation);
            
            if (testProjectDir != null)
            {
                // Tentar caminho relativo ao bin (Debug/Release) - onde o arquivo é copiado
                var appsettingsInBin = Path.Combine(testProjectDir, "appsettings.json");
                if (File.Exists(appsettingsInBin))
                {
                    config.AddJsonFile(appsettingsInBin, optional: false, reloadOnChange: false);
                }
                else
                {
                    // Tentar caminho relativo ao projeto (subindo 3 níveis de bin)
                    var appsettingsInProject = Path.Combine(testProjectDir, "..", "..", "..", "appsettings.json");
                    var normalizedPath = Path.GetFullPath(appsettingsInProject);
                    if (File.Exists(normalizedPath))
                    {
                        config.AddJsonFile(normalizedPath, optional: false, reloadOnChange: false);
                    }
                }
            }
        });

        // Criar um novo InMemoryDataStore isolado para esta instância do factory
        builder.ConfigureServices(services =>
        {
            // Remover o registro singleton existente do InMemoryDataStore
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(InMemoryDataStore));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Criar uma nova instância isolada para este teste
            _dataStore = new InMemoryDataStore();
            services.AddSingleton(_dataStore);

            // Os repositórios serão recriados automaticamente quando o container for construído,
            // pois eles dependem do InMemoryDataStore que acabamos de substituir
        });
    }

    /// <summary>
    /// Obtém o InMemoryDataStore usado nesta instância do factory.
    /// Útil para verificações diretas ou setup adicional nos testes.
    /// </summary>
    public InMemoryDataStore GetDataStore()
    {
        if (_dataStore == null)
        {
            _dataStore = Services.GetRequiredService<InMemoryDataStore>();
        }
        return _dataStore;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Limpar referência ao dataStore quando o factory for descartado
            _dataStore = null;
        }
        base.Dispose(disposing);
    }
}
