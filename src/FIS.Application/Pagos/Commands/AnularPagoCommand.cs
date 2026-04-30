using FIS.Contracts.Pagos;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Pagos.Commands;

public record AnularPagoCommand(AnularPagoRequest Request) : IRequest;

public class AnularPagoCommandHandler : IRequestHandler<AnularPagoCommand>
{
    private readonly IPagoRepository _repo;
    private readonly IUnitOfWork _uow;

    public AnularPagoCommandHandler(IPagoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Handle(AnularPagoCommand cmd, CancellationToken ct)
    {
        var pago = await _repo.GetByIdAsync(cmd.Request.IdPago, ct)
            ?? throw new BusinessException($"Pago con ID {cmd.Request.IdPago} no encontrado.");

        pago.Anular(cmd.Request.Motivo);
        _repo.Update(pago);
        await _uow.SaveChangesAsync(ct);
    }
}
