using System.Net;
using System.Text.Json;
using FIS.Contracts.Common;
using FIS.Domain.Common;
using FluentValidation;

namespace FIS.Api.Middleware;

/// <summary>
/// Convierte excepciones del dominio y de validación en respuestas HTTP coherentes.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (ValidationException vex)
        {
            _logger.LogWarning(vex, "Errores de validación");
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await WriteJson(ctx, ApiResponse<object>.ValidationFail(errors));
        }
        catch (BusinessException bex)
        {
            _logger.LogInformation(bex, "Regla de negocio violada");
            ctx.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            await WriteJson(ctx, ApiResponse<object>.Fail(bex.Message, bex.Code));
        }
        catch (UnauthorizedAccessException uex)
        {
            _logger.LogWarning(uex, "Acceso no autorizado");
            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await WriteJson(ctx, ApiResponse<object>.Fail("No autorizado", "UNAUTHORIZED"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado");
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteJson(ctx, ApiResponse<object>.Fail(
                "Ocurrió un error interno. Contacte al administrador.",
                "INTERNAL_ERROR"));
        }
    }

    private static Task WriteJson<T>(HttpContext ctx, T payload)
    {
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(payload,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
