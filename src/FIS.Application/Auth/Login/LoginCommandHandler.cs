using FIS.Application.Common.Interfaces;
using FIS.Contracts.Auth;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IUnitOfWork _uow;

    public LoginCommandHandler(
        IUsuarioRepository usuarios, IPasswordHasher hasher,
        IJwtTokenService jwt, IUnitOfWork uow)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<TokenResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var usuario = await _usuarios.GetByUsernameAsync(cmd.Username, ct)
            ?? throw new BusinessException("Credenciales inválidas.", "INVALID_CREDENTIALS");

        if (!usuario.Activo)
            throw new BusinessException("Usuario desactivado.", "USER_INACTIVE");

        if (usuario.EstaBloqueado())
            throw new BusinessException(
                $"Cuenta bloqueada hasta {usuario.BloqueadoHasta:HH:mm}. Intente luego.",
                "USER_LOCKED");

        if (!_hasher.Verify(cmd.Password, usuario.PasswordHash))
        {
            usuario.RegistrarIntentoFallido();
            _usuarios.Update(usuario);
            await _uow.SaveChangesAsync(ct);
            throw new BusinessException("Credenciales inválidas.", "INVALID_CREDENTIALS");
        }

        usuario.RegistrarLoginExitoso();
        _usuarios.Update(usuario);
        await _uow.SaveChangesAsync(ct);

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(usuario);
        var refreshToken = _jwt.GenerateRefreshToken();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserInfo
            {
                Id = usuario.IdUsuario,
                Username = usuario.Username,
                Email = usuario.Email,
                NombreCompleto = $"{usuario.Nombres} {usuario.Apellidos}".Trim(),
                Rol = usuario.Rol?.NombreRol ?? string.Empty
            }
        };
    }
}
