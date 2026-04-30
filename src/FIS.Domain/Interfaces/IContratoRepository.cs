using FIS.Domain.Entities;

namespace FIS.Domain.Interfaces;

public interface IContratoRepository
{
    Task<Contrato?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Contrato>> ListarPorClienteAsync(int idCliente, CancellationToken ct = default);
    Task<IReadOnlyList<Contrato>> ListarTodosAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> ContarTotalAsync(CancellationToken ct = default);
    Task<string> GenerarNumeroContratoAsync(CancellationToken ct = default);
    Task AddAsync(Contrato contrato, CancellationToken ct = default);
    void Update(Contrato contrato);
}
