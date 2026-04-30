using FIS.Domain.Common;
using FIS.Domain.Enums;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Contratos.Commands;

public record CambiarEstadoContratoCommand(int Id, string Accion) : IRequest;

public class CambiarEstadoContratoCommandHandler : IRequestHandler<CambiarEstadoContratoCommand>
{
    private readonly IContratoRepository _repo;
    private readonly IUnitOfWork _uow;

    public CambiarEstadoContratoCommandHandler(IContratoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Handle(CambiarEstadoContratoCommand cmd, CancellationToken ct)
    {
        var contrato = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new BusinessException($"Contrato con ID {cmd.Id} no encontrado.");

        switch (cmd.Accion.ToLower())
        {
            case "suspender": contrato.Suspender(); break;
            case "reactivar": contrato.Reactivar(); break;
            case "finalizar": contrato.Finalizar(); break;
            case "cancelar": contrato.Cancelar(); break;
            default: throw new BusinessException($"Acción '{cmd.Accion}' no reconocida.");
        }

        _repo.Update(contrato);
        await _uow.SaveChangesAsync(ct);
    }
}
