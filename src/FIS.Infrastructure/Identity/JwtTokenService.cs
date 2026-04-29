using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FIS.Application.Common.Interfaces;
using FIS.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FIS.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options) => _settings = options.Value;

    public (string accessToken, DateTime expiresAt) GenerateAccessToken(Usuario usuario)
    {
        var rolNombre = usuario.Rol?.NombreRol ?? string.Empty;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, usuario.Username),
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, rolNombre),
            new("nombres", usuario.Nombres),
            new("apellidos", usuario.Apellidos)
        };

        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
