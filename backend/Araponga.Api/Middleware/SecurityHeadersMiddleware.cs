namespace Araponga.Api.Middleware;

/// <summary>
/// Middleware para adicionar security headers em todas as respostas HTTP.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // X-Frame-Options: Previne clickjacking
        if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
        {
            context.Response.Headers.Append("X-Frame-Options", "DENY");
        }

        // X-Content-Type-Options: Previne MIME type sniffing
        if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        }

        // X-XSS-Protection: Proteção XSS (legacy, mas ainda útil)
        if (!context.Response.Headers.ContainsKey("X-XSS-Protection"))
        {
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        }

        // Referrer-Policy: Controla informações de referrer
        if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
        {
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        }

        // Permissions-Policy: Controla features do navegador
        if (!context.Response.Headers.ContainsKey("Permissions-Policy"))
        {
            context.Response.Headers.Append("Permissions-Policy", 
                "geolocation=(), microphone=(), camera=()");
        }

        // Content-Security-Policy: Política de segurança de conteúdo
        // Permite apenas recursos do mesmo origin e inline scripts/styles necessários
        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            var csp = "default-src 'self'; " +
                      "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                      "style-src 'self' 'unsafe-inline'; " +
                      "img-src 'self' data: https:; " +
                      "font-src 'self' data:; " +
                      "connect-src 'self'; " +
                      "frame-ancestors 'none';";
            
            context.Response.Headers.Append("Content-Security-Policy", csp);
        }

        await _next(context);
    }
}
