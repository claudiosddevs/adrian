using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExisteUsernameAsync(string username, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Usuario usuario, CancellationToken ct = default);
    void Update(Usuario usuario);
}
