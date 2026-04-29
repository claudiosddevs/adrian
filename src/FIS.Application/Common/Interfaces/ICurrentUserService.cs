namespace FIS.Application.Common.Interfaces;

/// <summary>
/// Provee acceso al usuario autenticado actual (resuelto desde el JWT).
/// La implementación vive en la capa API.
/// </summary>
public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Rol { get; }
    bool IsAuthenticated { get; }
}
