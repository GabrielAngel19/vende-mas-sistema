using Microsoft.AspNetCore.Mvc;

namespace Backend.Middleware;

public sealed class ApiExceptionMiddleware(
    RequestDelegate next,
    ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            await WriteProblemAsync(
                context,
                exception.StatusCode,
                exception.Title,
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error no controlado al procesar la solicitud.");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Error interno",
                "Ocurrió un error inesperado al procesar la solicitud.");
        }
    }

    private static Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path.Value
        });
    }
}
