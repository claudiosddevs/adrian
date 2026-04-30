using FIS.Application.Common.Interfaces;
using FIS.Contracts.Pagos;
using FIS.Domain.Common;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Pagos.Commands;

public record RegistrarPagoCommand(RegistrarPagoRequest Request) : IRequest<PagoDto>;

public class RegistrarPagoCommandHandler : IRequestHandler<RegistrarPagoCommand, PagoDto>
{
    private readonly IPagoRepository _pagoRepo;
    private readonly IContratoRepository _contratoRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public RegistrarPagoCommandHandler(
        IPagoRepository pagoRepo, IContratoRepository contratoRepo,
        IUnitOfWork uow, ICurrentUserService user)
    {
        _pagoRepo = pagoRepo;
        _contratoRepo = contratoRepo;
        _uow = uow;
        _user = user;
    }

    public async Task<PagoDto> Handle(RegistrarPagoCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var contrato = await _contratoRepo.GetByIdAsync(r.ContratoId, ct)
            ?? throw new BusinessException($"Contrato con ID {r.ContratoId} no encontrado.");

        var ahora = DateTime.UtcNow;
        var numero = await _pagoRepo.GenerarNumeroReciboAsync(ct);
        var mora = ahora.Day > 12 ? contrato.Plan?.PrecioMensual * 0.10m ?? 0 : 0;

        var pago = new Pago(
            r.ContratoId, _user.UserId ?? 0, r.MetodoPago, numero,
            (byte)ahora.Month, (short)ahora.Year,
            r.Monto, mora, null);

        await _pagoRepo.AddAsync(pago, ct);
        await _uow.SaveChangesAsync(ct);

        return new PagoDto
        {
            IdPago = pago.IdPago,
            ContratoId = pago.IdContrato,
            NombreCliente = contrato.Cliente?.NombreRazonSocial ?? string.Empty,
            FechaPago = pago.FechaPago,
            Monto = pago.MontoTotal,
            MetodoPago = pago.MetodoPago,
            Anulado = pago.Anulado,
            Mora = pago.MontoMora > 0 ? pago.MontoMora : null
        };
    }
}
