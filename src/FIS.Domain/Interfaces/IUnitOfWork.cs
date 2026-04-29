namespace FIS.Domain.Interfaces;

/// <summary>
/// Punto único de commit transaccional para los repositorios.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
