using Araponga.Bff.Journeys;
using Araponga.Bff.Middleware;
using Araponga.Bff.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BffOptions>(builder.Configuration.GetSection(BffOptions.SectionName));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IJourneyResponseCache, JourneyResponseCache>();

builder.Services.AddHttpClient<IJourneyApiProxy, JourneyApiProxy>(client =>
{
    var baseUrl = builder.Configuration["Bff:ApiBaseUrl"]?.TrimEnd('/');
    if (!string.IsNullOrEmpty(baseUrl))
        client.BaseAddress = new Uri(baseUrl);
});

// CORS: permite Flutter web (localhost com qualquer porta) e outros origins em dev
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;
                try
                {
                    var uri = new Uri(origin);
                    return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                }
                catch { return false; }
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Araponga BFF (Journeys)",
        Version = "v2",
        Description = "Backend for Frontend - Jornadas (onboarding, feed, eventos). Aplicação separada que encaminha para a API principal."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "BFF Journeys v2");
});

app.UseCors();

// Em Development com só HTTP (localhost:5001) evita aviso "Failed to determine the https port"
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseMiddleware<JourneyProxyMiddleware>();

// Página inicial indicativa (padrão enterprise: serviço rodando) - wwwroot/index.html
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Documentação de todas as jornadas expostas pelo BFF (proxy para a API)
app.MapGet("/bff/journeys", () =>
{
    var journeys = BffJourneyRegistry.AllPathPrefixes.Select(prefix =>
    {
        var basePath = $"{BffJourneyRegistry.BasePath}{prefix}";
        var endpoints = BffJourneyRegistry.AllEndpoints.TryGetValue(prefix, out var list)
            ? list.Select(e => new { e.Path, e.Method, e.Description }).ToList<object>()
            : new List<object>();
        return new { journey = prefix, basePath, endpoints };
    }).ToList();
    return Results.Ok(new { journeys });
})
.WithTags("BFF")
.WithName("GetBffJourneys");

app.Run();

/// <summary>Exposta para testes com WebApplicationFactory.</summary>
public partial class Program;
