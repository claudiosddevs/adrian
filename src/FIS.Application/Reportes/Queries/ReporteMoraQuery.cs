using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reportes;
using MediatR;

namespace FIS.Application.Reportes.Queries;

public record ReporteMoraQuery : IRequest<IReadOnlyList<ReporteMoraDto>>;

public class ReporteMoraQueryHandler : IRequestHandler<ReporteMoraQuery, IReadOnlyList<ReporteMoraDto>>
{
    private readonly IReporteService _svc;
    public ReporteMoraQueryHandler(IReporteService svc) => _svc = svc;

    public Task<IReadOnlyList<ReporteMoraDto>> Handle(ReporteMoraQuery q, CancellationToken ct)
        => _svc.GetReporteMoraAsync(ct);
}
