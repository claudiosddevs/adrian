using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reportes;
using MediatR;

namespace FIS.Application.Reportes.Queries;

public record BitacoraQuery(int Page = 1, int PageSize = 50, string? Tabla = null) : IRequest<IReadOnlyList<BitacoraDto>>;

public class BitacoraQueryHandler : IRequestHandler<BitacoraQuery, IReadOnlyList<BitacoraDto>>
{
    private readonly IReporteService _svc;
    public BitacoraQueryHandler(IReporteService svc) => _svc = svc;

    public Task<IReadOnlyList<BitacoraDto>> Handle(BitacoraQuery q, CancellationToken ct)
        => _svc.GetBitacoraAsync(q.Page, q.PageSize, q.Tabla, ct);
}
