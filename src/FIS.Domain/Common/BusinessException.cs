namespace FIS.Domain.Common;

/// <summary>
/// Excepción semántica para errores de reglas de negocio.
/// La capa API la traduce a HTTP 400 / 422.
/// </summary>
public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message, string code = "BUSINESS_ERROR")
        : base(message)
    {
        Code = code;
    }
}
