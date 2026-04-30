using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IPlanRepository
{
    Task<PlanServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PlanServicio>> ListarAsync(bool soloActivos = true, CancellationToken ct = default);
    Task AddAsync(PlanServicio plan, CancellationToken ct = default);
    void Update(PlanServicio plan);
}
