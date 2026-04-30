using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IReclamoRepository
{
    Task<Reclamo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Reclamo>> ListarAsync(string? estado, int? tecnicoId, int page, int pageSize, CancellationToken ct = default);
    Task<int> ContarTotalAsync(string? estado = null, CancellationToken ct = default);
    Task<int> ContarActivosPorTecnicoAsync(int tecnicoId, CancellationToken ct = default);
    Task<string> GenerarNumeroReclamoAsync(CancellationToken ct = default);
    Task AddAsync(Reclamo reclamo, CancellationToken ct = default);
    void Update(Reclamo reclamo);
}
