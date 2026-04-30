using FIS.Contracts.Planes;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Planes.Queries;

public record ListarPlanesQuery(bool SoloActivos = true) : IRequest<IReadOnlyList<PlanDto>>;

public class ListarPlanesQueryHandler : IRequestHandler<ListarPlanesQuery, IReadOnlyList<PlanDto>>
{
    private readonly IPlanRepository _repo;
    public ListarPlanesQueryHandler(IPlanRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PlanDto>> Handle(ListarPlanesQuery query, CancellationToken ct)
    {
        var planes = await _repo.ListarAsync(query.SoloActivos, ct);
        return planes.Select(p => new PlanDto
        {
            IdPlan = p.IdPlan,
            NombrePlan = p.NombrePlan,
            TipoServicio = p.TipoServicio,
            VelocidadBajadaMbps = p.VelocidadBajadaMbps,
            VelocidadSubidaMbps = p.VelocidadSubidaMbps,
            PrecioMensual = p.PrecioMensual,
            Activo = p.Activo
        }).ToList();
    }
}
