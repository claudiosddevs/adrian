using FIS.Domain.Entities;

namespace FIS.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string accessToken, DateTime expiresAt) GenerateAccessToken(Usuario usuario);
    string GenerateRefreshToken();
}
