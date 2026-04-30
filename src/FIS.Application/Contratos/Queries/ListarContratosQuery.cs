using FIS.Contracts.Common;
using FIS.Contracts.Contratos;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Contratos.Queries;

public record ListarContratosQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<ContratoDto>>;

public class ListarContratosQueryHandler : IRequestHandler<ListarContratosQuery, PagedResult<ContratoDto>>
{
    private readonly IContratoRepository _repo;
    public ListarContratosQueryHandler(IContratoRepository repo) => _repo = repo;

    public async Task<PagedResult<ContratoDto>> Handle(ListarContratosQuery q, CancellationToken ct)
    {
        var items = await _repo.ListarTodosAsync(q.Page, q.PageSize, ct);
        var total = await _repo.ContarTotalAsync(ct);
        return new PagedResult<ContratoDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    internal static ContratoDto MapToDto(Domain.Entities.Contrato c) => new()
    {
        IdContrato = c.IdContrato,
        ClienteId = c.IdCliente,
        NombreCliente = c.Cliente?.NombreRazonSocial ?? string.Empty,
        PlanId = c.IdPlan,
        NombrePlan = c.Plan?.NombrePlan ?? string.Empty,
        FechaInicio = c.FechaInicio,
        FechaFin = c.FechaFin,
        Estado = c.Estado,
        MetodoPago = "Mensual",
        MontoTotal = c.Plan?.PrecioMensual ?? 0
    };
}
