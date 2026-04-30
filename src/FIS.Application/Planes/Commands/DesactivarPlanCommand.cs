using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Planes.Commands;

public record DesactivarPlanCommand(int Id) : IRequest;

public class DesactivarPlanCommandHandler : IRequestHandler<DesactivarPlanCommand>
{
    private readonly IPlanRepository _repo;
    private readonly IUnitOfWork _uow;
    public DesactivarPlanCommandHandler(IPlanRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task Handle(DesactivarPlanCommand cmd, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new BusinessException($"Plan con ID {cmd.Id} no encontrado.");
        plan.Desactivar();
        _repo.Update(plan);
        await _uow.SaveChangesAsync(ct);
    }
}
