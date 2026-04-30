using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IPagoRepository
{
    Task<Pago?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> ListarPorContratoAsync(int idContrato, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> ListarTodosAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> ContarTotalAsync(CancellationToken ct = default);
    Task<string> GenerarNumeroReciboAsync(CancellationToken ct = default);
    Task AddAsync(Pago pago, CancellationToken ct = default);
    void Update(Pago pago);
}
