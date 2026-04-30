using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class ReclamoRepository : IReclamoRepository
{
    private readonly FisDbContext _ctx;
    public ReclamoRepository(FisDbContext ctx) => _ctx = ctx;

    public async Task<Reclamo?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Reclamos
            .Include(r => r.Cliente)
            .Include(r => r.Tecnico)
            .FirstOrDefaultAsync(r => r.IdReclamo == id, ct);

    public async Task<IReadOnlyList<Reclamo>> ListarAsync(
        string? estado, int? tecnicoId, int page, int pageSize, CancellationToken ct = default)
    {
        var q = _ctx.Reclamos
            .Include(r => r.Cliente)
            .Include(r => r.Tecnico)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
            q = q.Where(r => r.Estado == estado);
        if (tecnicoId.HasValue)
            q = q.Where(r => r.IdTecnico == tecnicoId);

        return await q
            .OrderByDescending(r => r.FechaApertura)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> ContarTotalAsync(string? estado = null, CancellationToken ct = default)
    {
        var q = _ctx.Reclamos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(estado))
            q = q.Where(r => r.Estado == estado);
        return await q.CountAsync(ct);
    }

    public async Task<int> ContarActivosPorTecnicoAsync(int tecnicoId, CancellationToken ct = default)
        => await _ctx.Reclamos.CountAsync(
            r => r.IdTecnico == tecnicoId &&
                 r.Estado != Domain.Enums.EstadoReclamo.Cerrado, ct);

    public async Task<string> GenerarNumeroReclamoAsync(CancellationToken ct = default)
    {
        var anio = DateTime.UtcNow.Year;
        var mes = DateTime.UtcNow.Month;
        var count = await _ctx.Reclamos.CountAsync(
            r => r.FechaApertura.Year == anio && r.FechaApertura.Month == mes, ct);
        return $"REC-{anio}{mes:D2}-{(count + 1):D4}";
    }

    public async Task AddAsync(Reclamo reclamo, CancellationToken ct = default)
        => await _ctx.Reclamos.AddAsync(reclamo, ct);

    public void Update(Reclamo reclamo)
        => _ctx.Entry(reclamo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
}
