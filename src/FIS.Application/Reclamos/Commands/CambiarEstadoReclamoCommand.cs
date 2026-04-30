using FIS.Contracts.Reclamos;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Reclamos.Commands;

public record CambiarEstadoReclamoCommand(int ReclamoId, CambiarEstadoReclamoRequest Request) : IRequest<ReclamoDto>;

public class CambiarEstadoReclamoCommandHandler : IRequestHandler<CambiarEstadoReclamoCommand, ReclamoDto>
{
    private readonly IReclamoRepository _repo;
    private readonly IUnitOfWork _uow;

    public CambiarEstadoReclamoCommandHandler(IReclamoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<ReclamoDto> Handle(CambiarEstadoReclamoCommand cmd, CancellationToken ct)
    {
        var reclamo = await _repo.GetByIdAsync(cmd.ReclamoId, ct)
            ?? throw new BusinessException($"Reclamo con ID {cmd.ReclamoId} no encontrado.");

        var r = cmd.Request;
        if (!string.IsNullOrWhiteSpace(r.Observacion) &&
            r.NuevoEstado == Domain.Enums.EstadoReclamo.Cerrado)
        {
            reclamo.RegistrarSolucion(r.Observacion);
        }
        else
        {
            reclamo.CambiarEstado(r.NuevoEstado);
        }

        _repo.Update(reclamo);
        await _uow.SaveChangesAsync(ct);

        return RegistrarReclamoCommandHandler.MapToDto(reclamo);
    }
}
