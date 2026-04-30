using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reportes;
using MediatR;

namespace FIS.Application.Reportes.Queries;

public record ReporteVentasQuery(int Anio) : IRequest<IReadOnlyList<ReporteVentasDto>>;

public class ReporteVentasQueryHandler : IRequestHandler<ReporteVentasQuery, IReadOnlyList<ReporteVentasDto>>
{
    private readonly IReporteService _svc;
    public ReporteVentasQueryHandler(IReporteService svc) => _svc = svc;

    public Task<IReadOnlyList<ReporteVentasDto>> Handle(ReporteVentasQuery q, CancellationToken ct)
        => _svc.GetReporteVentasAsync(q.Anio, ct);
}
