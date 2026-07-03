using Arah.Api.Extensions;
using Arah.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging estruturado (Serilog) — configurado no host.
builder.AddArahSerilog();

// Composition root: cada bloco de responsabilidade vive em uma extensão dedicada.
builder.Services
    .AddArahSecurity(builder.Configuration, builder.Environment)
    .AddArahObservability(builder.Configuration, builder.Environment)
    .AddArahRateLimiting(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddArahHealthChecks(builder.Configuration)
    .AddArahResponseCompression()
    .AddArahControllers()
    .AddArahCaching(builder.Configuration)
    .AddApplicationServices()
    .AddArahPaymentGateway(builder.Environment)
    .AddArahCore()
    .AddArahCoreSeedReleases()
    .AddEventHandlers()
    .AddArahSwagger();

var app = builder.Build();

app.UseArahPipeline();

try
{
    Log.Information("Starting Arah API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
