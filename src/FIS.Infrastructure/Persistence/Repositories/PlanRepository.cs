using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly FisDbContext _ctx;
    public PlanRepository(FisDbContext ctx) => _ctx = ctx;

    public async Task<PlanServicio?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Planes.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<PlanServicio>> ListarAsync(bool soloActivos = true, CancellationToken ct = default)
    {
        var q = _ctx.Planes.AsQueryable();
        if (soloActivos) q = q.Where(p => p.Activo);
        return await q.OrderBy(p => p.TipoServicio).ThenBy(p => p.PrecioMensual).ToListAsync(ct);
    }

    public async Task AddAsync(PlanServicio plan, CancellationToken ct = default)
        => await _ctx.Planes.AddAsync(plan, ct);

    public void Update(PlanServicio plan)
        => _ctx.Entry(plan).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
}
