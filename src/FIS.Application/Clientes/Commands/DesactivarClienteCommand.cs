using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Clientes.Commands;

public record DesactivarClienteCommand(int Id) : IRequest;

public class DesactivarClienteCommandHandler : IRequestHandler<DesactivarClienteCommand>
{
    private readonly IClienteRepository _repo;
    private readonly IUnitOfWork _uow;

    public DesactivarClienteCommandHandler(IClienteRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Handle(DesactivarClienteCommand cmd, CancellationToken ct)
    {
        var cliente = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new BusinessException($"Cliente con ID {cmd.Id} no encontrado.");

        cliente.Desactivar();
        _repo.Update(cliente);
        await _uow.SaveChangesAsync(ct);
    }
}
