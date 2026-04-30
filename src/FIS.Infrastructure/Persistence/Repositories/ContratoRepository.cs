using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class ContratoRepository : IContratoRepository
{
    private readonly FisDbContext _ctx;
    public ContratoRepository(FisDbContext ctx) => _ctx = ctx;

    public async Task<Contrato?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.IdContrato == id, ct);

    public async Task<IReadOnlyList<Contrato>> ListarPorClienteAsync(int idCliente, CancellationToken ct = default)
        => await _ctx.Contratos
            .Include(c => c.Plan)
            .Where(c => c.IdCliente == idCliente)
            .OrderByDescending(c => c.FechaInicio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Contrato>> ListarTodosAsync(int page, int pageSize, CancellationToken ct = default)
        => await _ctx.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Plan)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> ContarTotalAsync(CancellationToken ct = default)
        => await _ctx.Contratos.CountAsync(ct);

    public async Task<string> GenerarNumeroContratoAsync(CancellationToken ct = default)
    {
        var anio = DateTime.UtcNow.Year;
        var count = await _ctx.Contratos.CountAsync(c => c.FechaInicio.Year == anio, ct);
        return $"CTR-{anio}-{(count + 1):D4}";
    }

    public async Task AddAsync(Contrato contrato, CancellationToken ct = default)
        => await _ctx.Contratos.AddAsync(contrato, ct);

    public void Update(Contrato contrato)
        => _ctx.Entry(contrato).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
}
