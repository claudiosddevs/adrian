using FIS.Contracts.Reclamos;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Reclamos.Commands;

public record AsignarTecnicoCommand(int ReclamoId, AsignarTecnicoRequest Request) : IRequest<ReclamoDto>;

public class AsignarTecnicoCommandHandler : IRequestHandler<AsignarTecnicoCommand, ReclamoDto>
{
    private readonly IReclamoRepository _repo;
    private readonly IUnitOfWork _uow;

    public AsignarTecnicoCommandHandler(IReclamoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<ReclamoDto> Handle(AsignarTecnicoCommand cmd, CancellationToken ct)
    {
        var reclamo = await _repo.GetByIdAsync(cmd.ReclamoId, ct)
            ?? throw new BusinessException($"Reclamo con ID {cmd.ReclamoId} no encontrado.");

        var activos = await _repo.ContarActivosPorTecnicoAsync(cmd.Request.TecnicoId, ct);
        reclamo.AsignarTecnico(cmd.Request.TecnicoId, activos);

        _repo.Update(reclamo);
        await _uow.SaveChangesAsync(ct);

        return RegistrarReclamoCommandHandler.MapToDto(reclamo);
    }
}
