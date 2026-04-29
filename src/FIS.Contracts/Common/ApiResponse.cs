namespace FIS.Contracts.Common;

/// <summary>
/// Sobre estándar de respuesta de la API. Todas las respuestas la usan para
/// proveer una forma consistente al cliente.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    public static ApiResponse<T> Ok(T? data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, string? errorCode = null) =>
        new() { Success = false, Message = message, ErrorCode = errorCode };

    public static ApiResponse<T> ValidationFail(Dictionary<string, string[]> errors) =>
        new()
        {
            Success = false,
            Message = "Errores de validación",
            ErrorCode = "VALIDATION_ERROR",
            ValidationErrors = errors
        };
}
