using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reclamos;
using FIS.Domain.Common;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Reclamos.Commands;

public record RegistrarReclamoCommand(RegistrarReclamoRequest Request) : IRequest<ReclamoDto>;

public class RegistrarReclamoCommandHandler : IRequestHandler<RegistrarReclamoCommand, ReclamoDto>
{
    private readonly IReclamoRepository _repo;
    private readonly IClienteRepository _clienteRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public RegistrarReclamoCommandHandler(
        IReclamoRepository repo, IClienteRepository clienteRepo,
        IUnitOfWork uow, ICurrentUserService user)
    {
        _repo = repo;
        _clienteRepo = clienteRepo;
        _uow = uow;
        _user = user;
    }

    public async Task<ReclamoDto> Handle(RegistrarReclamoCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var cliente = await _clienteRepo.GetByIdAsync(r.ClienteId, ct)
            ?? throw new BusinessException($"Cliente con ID {r.ClienteId} no encontrado.");

        var numero = await _repo.GenerarNumeroReclamoAsync(ct);
        var reclamo = new Reclamo(
            r.ClienteId, null, _user.UserId ?? 0,
            numero, r.Clasificacion,
            r.Descripcion, 3, Domain.Enums.CanalEntrada.Telefono);

        await _repo.AddAsync(reclamo, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(reclamo, cliente.NombreRazonSocial);
    }

    internal static ReclamoDto MapToDto(Reclamo r, string nombreCliente = "", string? nombreTecnico = null) => new()
    {
        IdReclamo = r.IdReclamo,
        ClienteId = r.IdCliente,
        NombreCliente = r.Cliente?.NombreRazonSocial ?? nombreCliente,
        Descripcion = r.DescripcionProblema,
        Clasificacion = r.TipoReclamo,
        Estado = r.Estado,
        TecnicoAsignadoId = r.IdTecnico,
        NombreTecnico = r.Tecnico is not null ? $"{r.Tecnico.Nombres} {r.Tecnico.Apellidos}" : nombreTecnico,
        FechaRegistro = r.FechaApertura,
        FechaCierre = r.FechaCierre,
        ObservacionCierre = r.SolucionAplicada,
        Calificacion = r.Calificacion
    };
}
