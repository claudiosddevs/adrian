using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class PagoRepository : IPagoRepository
{
    private readonly FisDbContext _ctx;
    public PagoRepository(FisDbContext ctx) => _ctx = ctx;

    public async Task<Pago?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Pagos
            .Include(p => p.Contrato).ThenInclude(c => c.Cliente)
            .FirstOrDefaultAsync(p => p.IdPago == id, ct);

    public async Task<IReadOnlyList<Pago>> ListarPorContratoAsync(int idContrato, CancellationToken ct = default)
        => await _ctx.Pagos
            .Where(p => p.IdContrato == idContrato)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Pago>> ListarTodosAsync(int page, int pageSize, CancellationToken ct = default)
        => await _ctx.Pagos
            .Include(p => p.Contrato).ThenInclude(c => c.Cliente)
            .OrderByDescending(p => p.FechaPago)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> ContarTotalAsync(CancellationToken ct = default)
        => await _ctx.Pagos.CountAsync(ct);

    public async Task<string> GenerarNumeroReciboAsync(CancellationToken ct = default)
    {
        var anio = DateTime.UtcNow.Year;
        var count = await _ctx.Pagos.CountAsync(p => p.FechaPago.Year == anio, ct);
        return $"REC-{anio}-{(count + 1):D6}";
    }

    public async Task AddAsync(Pago pago, CancellationToken ct = default)
        => await _ctx.Pagos.AddAsync(pago, ct);

    public void Update(Pago pago)
        => _ctx.Entry(pago).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
}
