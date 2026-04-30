using FIS.Contracts.Common;
using FIS.Contracts.Pagos;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Pagos.Queries;

public record ListarPagosQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<PagoDto>>;

public class ListarPagosQueryHandler : IRequestHandler<ListarPagosQuery, PagedResult<PagoDto>>
{
    private readonly IPagoRepository _repo;
    public ListarPagosQueryHandler(IPagoRepository repo) => _repo = repo;

    public async Task<PagedResult<PagoDto>> Handle(ListarPagosQuery q, CancellationToken ct)
    {
        var items = await _repo.ListarTodosAsync(q.Page, q.PageSize, ct);
        var total = await _repo.ContarTotalAsync(ct);

        return new PagedResult<PagoDto>
        {
            Items = items.Select(p => new PagoDto
            {
                IdPago = p.IdPago,
                ContratoId = p.IdContrato,
                NombreCliente = p.Contrato?.Cliente?.NombreRazonSocial ?? string.Empty,
                FechaPago = p.FechaPago,
                Monto = p.MontoTotal,
                MetodoPago = p.MetodoPago,
                Anulado = p.Anulado,
                MotivoAnulacion = p.MotivoAnulacion,
                Mora = p.MontoMora > 0 ? p.MontoMora : null
            }).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }
}
