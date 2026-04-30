using FIS.Contracts.Planes;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Planes.Commands;

public record ActualizarPlanCommand(int Id, CrearPlanRequest Request) : IRequest<PlanDto>;

public class ActualizarPlanCommandHandler : IRequestHandler<ActualizarPlanCommand, PlanDto>
{
    private readonly IPlanRepository _repo;
    private readonly IUnitOfWork _uow;
    public ActualizarPlanCommandHandler(IPlanRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<PlanDto> Handle(ActualizarPlanCommand cmd, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new BusinessException($"Plan con ID {cmd.Id} no encontrado.");

        var r = cmd.Request;
        plan.Actualizar(r.NombrePlan, r.TipoServicio, r.VelocidadBajadaMbps, r.VelocidadSubidaMbps, r.PrecioMensual);
        _repo.Update(plan);
        await _uow.SaveChangesAsync(ct);
        return CrearPlanCommandHandler.MapToDto(plan);
    }
}
