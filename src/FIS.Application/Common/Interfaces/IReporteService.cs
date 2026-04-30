using FIS.Contracts.Reportes;

namespace FIS.Application.Common.Interfaces;

public interface IReporteService
{
    Task<IReadOnlyList<ReporteMoraDto>> GetReporteMoraAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ReporteVentasDto>> GetReporteVentasAsync(int anio, CancellationToken ct = default);
    Task<IReadOnlyList<ReporteTecnicoDto>> GetReporteTecnicosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BitacoraDto>> GetBitacoraAsync(int page, int pageSize, string? tabla, CancellationToken ct = default);
}
