using System.Security.Claims;
using FIS.Application.Common.Interfaces;

namespace FIS.Api.Identity;

/// <summary>
/// Implementación del usuario actual basada en HttpContext + JWT claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public int? UserId
    {
        get
        {
            var raw = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? Username =>
        _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Rol =>
        _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
