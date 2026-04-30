using FIS.Application.Common.Interfaces;
using FIS.Contracts.Usuarios;
using FIS.Domain.Common;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Usuarios.Commands;

public record CrearUsuarioCommand(CrearUsuarioRequest Request) : IRequest<UsuarioDto>;

public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public CrearUsuarioCommandHandler(IUsuarioRepository repo, IPasswordHasher hasher, IUnitOfWork uow)
    {
        _repo = repo;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task<UsuarioDto> Handle(CrearUsuarioCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;

        if (await _repo.ExisteUsernameAsync(r.NombreUsuario, ct))
            throw new BusinessException($"El nombre de usuario '{r.NombreUsuario}' ya está en uso.");

        if (await _repo.ExisteEmailAsync(r.Email, ct))
            throw new BusinessException($"El email '{r.Email}' ya está registrado.");

        var partes = r.NombreCompleto.Trim().Split(' ', 2);
        var nombres = partes[0];
        var apellidos = partes.Length > 1 ? partes[1] : string.Empty;

        var hash = _hasher.Hash(r.Password);
        var usuario = new Usuario((byte)r.RolId, r.NombreUsuario, hash, r.Email, nombres, apellidos);

        await _repo.AddAsync(usuario, ct);
        await _uow.SaveChangesAsync(ct);

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.Username,
            NombreCompleto = $"{usuario.Nombres} {usuario.Apellidos}",
            Email = usuario.Email,
            Rol = string.Empty,
            Activo = usuario.Activo,
            FechaCreacion = usuario.CreatedAt
        };
    }
}
