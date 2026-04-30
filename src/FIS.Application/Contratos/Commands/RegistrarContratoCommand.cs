using FIS.Application.Common.Interfaces;
using FIS.Contracts.Contratos;
using FIS.Domain.Common;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Contratos.Commands;

public record RegistrarContratoCommand(RegistrarContratoRequest Request) : IRequest<ContratoDto>;

public class RegistrarContratoCommandHandler : IRequestHandler<RegistrarContratoCommand, ContratoDto>
{
    private readonly IContratoRepository _contratoRepo;
    private readonly IClienteRepository _clienteRepo;
    private readonly IPlanRepository _planRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public RegistrarContratoCommandHandler(
        IContratoRepository contratoRepo, IClienteRepository clienteRepo,
        IPlanRepository planRepo, IUnitOfWork uow, ICurrentUserService user)
    {
        _contratoRepo = contratoRepo;
        _clienteRepo = clienteRepo;
        _planRepo = planRepo;
        _uow = uow;
        _user = user;
    }

    public async Task<ContratoDto> Handle(RegistrarContratoCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var cliente = await _clienteRepo.GetByIdAsync(r.ClienteId, ct)
            ?? throw new BusinessException($"Cliente con ID {r.ClienteId} no encontrado.");
        var plan = await _planRepo.GetByIdAsync(r.PlanId, ct)
            ?? throw new BusinessException($"Plan con ID {r.PlanId} no encontrado.");

        var numero = await _contratoRepo.GenerarNumeroContratoAsync(ct);
        var contrato = new Contrato(
            r.ClienteId, r.PlanId, _user.UserId ?? 0,
            numero, r.FechaInicio, r.FechaFin);

        await _contratoRepo.AddAsync(contrato, ct);
        await _uow.SaveChangesAsync(ct);

        return new ContratoDto
        {
            IdContrato = contrato.IdContrato,
            ClienteId = contrato.IdCliente,
            NombreCliente = cliente.NombreRazonSocial,
            PlanId = contrato.IdPlan,
            NombrePlan = plan.NombrePlan,
            FechaInicio = contrato.FechaInicio,
            FechaFin = contrato.FechaFin,
            Estado = contrato.Estado,
            MetodoPago = r.MetodoPago,
            MontoTotal = plan.PrecioMensual
        };
    }
}
