using Arah.Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Arah.Api.Extensions;

/// <summary>
/// Handler global de exceções: mapeia exceções de aplicação/domínio para códigos HTTP
/// e produz uma resposta ProblemDetails consistente.
/// </summary>
public static class ExceptionHandlingExtensions
{
    public static WebApplication UseArahExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature?.Error;
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                if (exception is not null)
                {
                    logger.LogError(exception, "Unhandled exception at {Path}", feature?.Path);
                }

                var includeDetails = app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");
                var statusCode = exception switch
                {
                    Arah.Application.Exceptions.ValidationException => StatusCodes.Status400BadRequest,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    ConflictException => StatusCodes.Status409Conflict,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    InvalidOperationException => StatusCodes.Status409Conflict,
                    Arah.Application.Exceptions.DomainException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var detail = "An unexpected error occurred.";
                if (includeDetails && exception is not null)
                {
                    detail = exception.Message;
                    if (exception.InnerException is not null)
                        detail += " | Inner: " + exception.InnerException.Message;
                }
                var problem = new ProblemDetails
                {
                    Title = "Unexpected error",
                    Status = statusCode,
                    Detail = detail,
                    Instance = feature?.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;
                problem.Extensions["path"] = feature?.Path;
                if (includeDetails && exception is not null)
                {
                    problem.Extensions["exceptionType"] = exception.GetType().FullName;
                    problem.Extensions["stackTrace"] = exception.StackTrace;
                    if (exception.InnerException is not null)
                    {
                        problem.Extensions["innerException"] = exception.InnerException.Message;
                    }
                }

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                // Serializar manualmente para garantir que path e traceId sejam propriedades diretas
                var response = new Dictionary<string, object?>
                {
                    ["title"] = problem.Title ?? "Unexpected error",
                    ["status"] = problem.Status ?? statusCode,
                    ["detail"] = problem.Detail,
                    ["instance"] = problem.Instance ?? feature?.Path,
                    ["traceId"] = context.TraceIdentifier,
                    ["path"] = feature?.Path
                };

                await context.Response.WriteAsJsonAsync(response);
            });
        });

        return app;
    }
}
