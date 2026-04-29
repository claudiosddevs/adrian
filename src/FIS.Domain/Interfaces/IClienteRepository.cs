using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(IReadOnlyList<Cliente> items, int total)> ListarAsync(
        string? filtro, int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExisteNitCiAsync(string nitCi, CancellationToken ct = default);
    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    void Update(Cliente cliente);
}
