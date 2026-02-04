using System.Collections.Generic;
using Araponga.Api;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
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
    private InMemorySharedStore? _sharedStore;

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

            // Substituir também o InMemorySharedStore para que testes e API usem a mesma instância
            var sharedDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(InMemorySharedStore));
            if (sharedDescriptor != null) services.Remove(sharedDescriptor);
            _sharedStore = new InMemorySharedStore();
            services.AddSingleton(_sharedStore);

            // Criar uma nova instância isolada para este teste
            _dataStore = new InMemoryDataStore();
            services.AddSingleton(_dataStore);

            // Os repositórios serão recriados automaticamente quando o container for construído,
            // pois eles dependem do InMemoryDataStore/InMemorySharedStore que acabamos de substituir
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

    /// <summary>
    /// Obtém o InMemorySharedStore usado nesta instância do factory (Users, Memberships, etc.).
    /// Use para setup de dados shared em testes BDD.
    /// </summary>
    public InMemorySharedStore GetSharedStore()
    {
        if (_sharedStore == null)
        {
            _sharedStore = Services.GetRequiredService<InMemorySharedStore>();
        }
        return _sharedStore;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dataStore = null;
            _sharedStore = null;
        }
        base.Dispose(disposing);
    }
}
