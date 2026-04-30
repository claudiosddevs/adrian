using FIS.Contracts.Planes;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Planes.Commands;

public record CrearPlanCommand(CrearPlanRequest Request) : IRequest<PlanDto>;

public class CrearPlanCommandHandler : IRequestHandler<CrearPlanCommand, PlanDto>
{
    private readonly IPlanRepository _repo;
    private readonly IUnitOfWork _uow;
    public CrearPlanCommandHandler(IPlanRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<PlanDto> Handle(CrearPlanCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var plan = new PlanServicio(r.NombrePlan, r.TipoServicio, r.VelocidadBajadaMbps, r.VelocidadSubidaMbps, r.PrecioMensual);
        await _repo.AddAsync(plan, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToDto(plan);
    }

    internal static PlanDto MapToDto(PlanServicio p) => new()
    {
        IdPlan = p.IdPlan,
        NombrePlan = p.NombrePlan,
        TipoServicio = p.TipoServicio,
        VelocidadBajadaMbps = p.VelocidadBajadaMbps,
        VelocidadSubidaMbps = p.VelocidadSubidaMbps,
        PrecioMensual = p.PrecioMensual,
        Activo = p.Activo
    };
}
