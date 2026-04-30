using FIS.Application.Common.Interfaces;
using FIS.Contracts.Clientes;
using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Clientes.Commands;

public record CrearClienteCommand(CrearClienteRequest Request) : IRequest<ClienteDto>;

public class CrearClienteCommandHandler : IRequestHandler<CrearClienteCommand, ClienteDto>
{
    private readonly IClienteRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;

    public CrearClienteCommandHandler(IClienteRepository repo, IUnitOfWork uow, ICurrentUserService user)
    {
        _repo = repo;
        _uow = uow;
        _user = user;
    }

    public async Task<ClienteDto> Handle(CrearClienteCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;

        if (await _repo.ExisteNitCiAsync(req.NitCi, ct))
            throw new Domain.Common.BusinessException($"Ya existe un cliente con NIT/CI '{req.NitCi}'.");

        var count = (await _repo.ListarAsync(null, 1, int.MaxValue, ct)).total + 1;
        var codigo = $"CLI-{count:D5}";

        var cliente = new Cliente(
            req.TipoCliente, codigo, req.NombreRazonSocial,
            req.NitCi, req.Email, req.Telefono, req.Direccion, req.Ciudad);

        await _repo.AddAsync(cliente, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(cliente);
    }

    internal static ClienteDto MapToDto(Cliente c) => new()
    {
        IdCliente = c.IdCliente,
        TipoCliente = c.TipoCliente,
        CodigoCliente = c.CodigoCliente,
        NombreRazonSocial = c.NombreRazonSocial,
        NitCi = c.NitCi,
        Email = c.Email,
        Telefono = c.Telefono,
        Direccion = c.Direccion,
        Ciudad = c.Ciudad,
        Activo = c.Activo,
        FechaRegistro = c.FechaRegistro
    };
}
