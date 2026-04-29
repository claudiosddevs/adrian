using System.Security.Cryptography;
using System.Text;

namespace FIS.Desktop.Services;

/// <summary>
/// Almacena el token JWT en memoria. En producción, persistir el refresh
/// token cifrado con DPAPI a un archivo del usuario actual.
/// </summary>
public class TokenStore
{
    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt;

    public string? AccessToken => _accessToken;
    public bool IsAuthenticated =>
        !string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTime.UtcNow;

    public void Save(string accessToken, string refreshToken, DateTime expiresAt)
    {
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _expiresAt = expiresAt;
    }

    public void Clear()
    {
        _accessToken = null;
        _refreshToken = null;
        _expiresAt = DateTime.MinValue;
    }

    /// <summary>
    /// Cifra una cadena con DPAPI (CurrentUser scope) para persistencia segura.
    /// Útil al guardar el refresh token en disco entre sesiones.
    /// </summary>
    public static string Protect(string plain)
    {
        if (!OperatingSystem.IsWindows()) return plain;
        var bytes = Encoding.UTF8.GetBytes(plain);
        var protected_ = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protected_);
    }

    public static string Unprotect(string encoded)
    {
        if (!OperatingSystem.IsWindows()) return encoded;
        var bytes = Convert.FromBase64String(encoded);
        var clear = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(clear);
    }
}
