using Araponga.Application;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Admin;

public sealed class AdminModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Criar AdminDbContext e entidades quando necessário
        // Por enquanto, Admin pode usar SharedDbContext ou ArapongaDbContext temporariamente
    }
}
