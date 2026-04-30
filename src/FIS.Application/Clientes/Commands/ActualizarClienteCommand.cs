using FIS.Contracts.Clientes;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Clientes.Commands;

public record ActualizarClienteCommand(int Id, CrearClienteRequest Request) : IRequest<ClienteDto>;

public class ActualizarClienteCommandHandler : IRequestHandler<ActualizarClienteCommand, ClienteDto>
{
    private readonly IClienteRepository _repo;
    private readonly IUnitOfWork _uow;

    public ActualizarClienteCommandHandler(IClienteRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<ClienteDto> Handle(ActualizarClienteCommand cmd, CancellationToken ct)
    {
        var cliente = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new BusinessException($"Cliente con ID {cmd.Id} no encontrado.");

        cliente.Actualizar(
            cmd.Request.NombreRazonSocial,
            cmd.Request.Email,
            cmd.Request.Telefono,
            cmd.Request.Direccion,
            cmd.Request.Ciudad);

        _repo.Update(cliente);
        await _uow.SaveChangesAsync(ct);

        return CrearClienteCommandHandler.MapToDto(cliente);
    }
}
