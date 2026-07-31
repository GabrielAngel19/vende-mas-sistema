namespace Backend.Middleware;

public class ApiException(int statusCode, string title, string message)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
}

public sealed class NotFoundException(string message)
    : ApiException(StatusCodes.Status404NotFound, "Recurso no encontrado", message);

public sealed class ConflictException(string message)
    : ApiException(StatusCodes.Status409Conflict, "Conflicto", message);

public sealed class BusinessRuleException(string message)
    : ApiException(StatusCodes.Status400BadRequest, "Operación inválida", message);
