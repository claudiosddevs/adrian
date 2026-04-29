using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly FisDbContext _db;

    public UsuarioRepository(FisDbContext db) => _db = db;

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Usuarios.Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id, ct);

    public Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        _db.Usuarios.Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Usuarios.Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExisteUsernameAsync(string username, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Username == username, ct);

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(Usuario usuario, CancellationToken ct = default) =>
        await _db.Usuarios.AddAsync(usuario, ct);

    public void Update(Usuario usuario) => _db.Usuarios.Update(usuario);
}
