using FIS.Contracts.Usuarios;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Usuarios.Queries;

public record ListarUsuariosQuery : IRequest<IReadOnlyList<UsuarioDto>>;

public class ListarUsuariosQueryHandler : IRequestHandler<ListarUsuariosQuery, IReadOnlyList<UsuarioDto>>
{
    private readonly IUsuarioRepository _repo;
    public ListarUsuariosQueryHandler(IUsuarioRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<UsuarioDto>> Handle(ListarUsuariosQuery q, CancellationToken ct)
    {
        var usuarios = await _repo.ListarTodosAsync(ct);
        return usuarios.Select(u => new UsuarioDto
        {
            IdUsuario = u.IdUsuario,
            NombreUsuario = u.Username,
            NombreCompleto = $"{u.Nombres} {u.Apellidos}",
            Email = u.Email,
            Rol = u.Rol?.NombreRol ?? string.Empty,
            Activo = u.Activo,
            FechaCreacion = u.CreatedAt
        }).ToList();
    }
}
