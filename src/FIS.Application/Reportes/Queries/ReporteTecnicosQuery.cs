using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reportes;
using MediatR;

namespace FIS.Application.Reportes.Queries;

public record ReporteTecnicosQuery : IRequest<IReadOnlyList<ReporteTecnicoDto>>;

public class ReporteTecnicosQueryHandler : IRequestHandler<ReporteTecnicosQuery, IReadOnlyList<ReporteTecnicoDto>>
{
    private readonly IReporteService _svc;
    public ReporteTecnicosQueryHandler(IReporteService svc) => _svc = svc;

    public Task<IReadOnlyList<ReporteTecnicoDto>> Handle(ReporteTecnicosQuery q, CancellationToken ct)
        => _svc.GetReporteTecnicosAsync(ct);
}
