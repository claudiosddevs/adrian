using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly FisDbContext _db;

    public ClienteRepository(FisDbContext db) => _db = db;

    public Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Clientes.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdCliente == id, ct);

    public async Task<(IReadOnlyList<Cliente> items, int total)> ListarAsync(
        string? filtro, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Clientes.AsNoTracking().Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var f = $"%{filtro}%";
            query = query.Where(c =>
                EF.Functions.Like(c.NombreRazonSocial, f) ||
                EF.Functions.Like(c.NitCi, f) ||
                EF.Functions.Like(c.Email, f) ||
                EF.Functions.Like(c.CodigoCliente, f));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(c => c.NombreRazonSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<bool> ExisteNitCiAsync(string nitCi, CancellationToken ct = default) =>
        _db.Clientes.AnyAsync(c => c.NitCi == nitCi, ct);

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default) =>
        await _db.Clientes.AddAsync(cliente, ct);

    public void Update(Cliente cliente) => _db.Clientes.Update(cliente);
}
